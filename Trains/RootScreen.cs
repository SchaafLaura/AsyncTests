using System.Xml;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace MyRss;

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
            Position = ((GameSettings.GAME_WIDTH-("Load").Length)/2, GameSettings.GAME_HEIGHT-1)
        };
        btn.Click += (_, _) =>
        {
            var k = 0;
            foreach(var c in Children)
                if (c is FeedSurface f)
                    f.SetTask(GetFeed(_feedUrLs[k++]));
        };
        Controls.Add(btn);
    }

    private void InitFeeds()
    {
        var k = 0;
        foreach (var _ in _feedUrLs)
            Children.Add(new FeedSurface(GameSettings.GAME_WIDTH - 2, 5){ Position = (1, (k++) * 6) });
    }

    public async Task<Feed?> GetFeed(string feedURL)
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
                Title:  t[0]!.InnerText,
                Newest: t[1]!.InnerText,
                Second: t[2]!.InnerText);
        }
        catch (System.Exception e)
        {
            return null;
        }
    }
}