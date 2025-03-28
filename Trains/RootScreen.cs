using System.Xml;
using LanguageExt;
using LanguageExt.Common;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace MyRss;

class RootScreen : ControlsConsole
{
    private HttpClient _client = new();
    private FeedSurface[] _feedSurfaces;
    
    private string[] _feedUrLs =
    [
        "https://podcastfeeds.nbcnews.com/RPWEjhKq",
        "https://godotengine.org/rss.xml",
        "httpsa://VeryWrongURL.XYZ",
        "https://www.wired.com/feed/rss",
        "https://feeds.content.dowjones.io/public/rss/RSSOpinion",
    ];
    
    public RootScreen() : base(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    {
        _feedSurfaces = new FeedSurface[_feedUrLs.Length];
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
            foreach (var f in _feedSurfaces)
                f.LoadFeedAsync(GetFeed(_feedUrLs[k++]));
        };
        Controls.Add(btn);
    }

    private void InitFeeds()
    {
        var k = 0;
        foreach (var _ in _feedUrLs)
        {
            var f = new FeedSurface(GameSettings.GAME_WIDTH - 2, 5)
            {
                Position = (1, k * 6)
            };
            Children.Add(f);
            _feedSurfaces[k++] = f;
        }
    }
    
    public async Task<Fin<Feed>> GetFeed(string feedURL)
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
                Second: t[2]!.InnerText,
                URL:    feedURL);
        }
        catch (Exception e)
        {
            return Error.New(e.Message + $" (url:{feedURL})");
        }
    }
}