namespace MyRss;
internal sealed class FeedSurface(int width, int height, Task<Feed>? task) : ScreenSurface(width, height)
{
    public Task<Feed>? Task = task;
    private bool _written;
    private TimeSpan _timeSinceLastDraw = TimeSpan.Zero;
    private TimeSpan _drawThreshold = new (1_000_000);
    
    private int _k = 0;
    public override void Update(TimeSpan delta)
    {
        if (!_written)
        {
            if (_timeSinceLastDraw > _drawThreshold)
            {
                Surface.Clear(new Rectangle(0, 0, (_k % 3) + 1, 1));
                Surface.Print(0, 0, new string('.', (++_k % 3) + 1));
                _timeSinceLastDraw = TimeSpan.Zero;
            }
            else
                _timeSinceLastDraw += delta;

            TryWrite();
        }

        base.Update(delta);
    }

    private void TryWrite()
    {
        if (Task is null || !Task.IsCompleted)
            return;
        _written = true;
        var f = Task.Result;
        Surface.Clear(new Rectangle(0, 0, (_k % 3) + 1, 1));
        Surface.Print(1, 0, f.Title);
        Surface.Print(3, 1, f.Newest);
        Surface.Print(3, 2, f.Second);
    }
}