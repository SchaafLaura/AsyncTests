using System.Xml;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace MyRss;

internal sealed class Feed(string title, string newest, string second)
{
    public string Title = title;
    public string Newest = newest;
    public string Second = second;
}

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

class RootScreen : ControlsConsole
{
    private HttpClient _client = new();
    
    private string[] _feedUrLs =
    [
        "https://podcastfeeds.nbcnews.com/RPWEjhKq",
        "https://godotengine.org/rss.xml",
        "https://www.dailymail.co.uk/news/transgender-issues/index.rss",
        "https://www.wired.com/feed/rss",
        "https://feeds.content.dowjones.io/public/rss/RSSOpinion",
    ];
    
    public RootScreen() : base(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    {
        InitFeeds();
        InitLoadBtn();
    }

    private void InitLoadBtn()
    {
        var btn = new Button("Load")
        {
            Position = ((GameSettings.GAME_WIDTH-("load").Length)/2, GameSettings.GAME_HEIGHT-1)
        };
        btn.Click += (_, _) =>
        {
            var k = 0;
            foreach(var c in Children)
                if (c is FeedSurface f)
                    f.Task = GetFeed(_feedUrLs[k++]);
        };
        Controls.Add(btn);
    }

    private void InitFeeds()
    {
        var k = 0;
        foreach (var _ in _feedUrLs)
            Children.Add(new FeedSurface(GameSettings.GAME_WIDTH - 2, 5, null){ Position = (1, (k++) * 6) });
    }

    public async Task<Feed> GetFeed(string feedURL)
    {
        try
        {
            using var response = await _client.GetAsync(new Uri(feedURL));
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(body);
            var t = xmlDoc.GetElementsByTagName("title");
            
            return new Feed(
                title:  t[0]!.InnerText,
                newest: t[1]!.InnerText,
                second: t[2]!.InnerText);
        }
        catch (System.Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
}