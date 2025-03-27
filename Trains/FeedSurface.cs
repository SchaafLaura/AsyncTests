using LanguageExt;

namespace MyRss;
internal sealed class FeedSurface(int width, int height) : ScreenSurface(width, height)
{
    private bool _written;
    private TimeSpan _timeSinceLastDraw = TimeSpan.Zero;
    private TimeSpan _drawThreshold = new(1_000_000);
    
    private int _k;
    public override void Update(TimeSpan delta)
    {
        if (!_written)
            DrawDots(delta);
        
        base.Update(delta);
    }

    private void DrawDots(TimeSpan delta)
    {
        if (_timeSinceLastDraw > _drawThreshold)
        {
            Surface.Clear(new Rectangle(0, 0, (_k % 3) + 1, 1));
            Surface.Print(0, 0, new string('.', (++_k % 3) + 1));
            _timeSinceLastDraw = TimeSpan.Zero;
        }
        else
            _timeSinceLastDraw += delta; 
    }
    
    public async void SetTask(Task<Fin<Feed>> task) =>
        (await task).Match(
            feed => WriteFeedToSurface(feed.Title, feed.Newest, feed.Second),
            err  => WriteFeedToSurface(err.Message, "???", "???"));
    
    private void WriteFeedToSurface(string title, string newest, string second)
    {
        _written = true;
        Surface.Clear(new Rectangle(0, 0, (_k % 3) + 1, 1));
        Surface.Print(1, 0, title);
        Surface.Print(3, 1, newest);
        Surface.Print(3, 2, second);
    }
}