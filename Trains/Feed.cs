namespace MyRss;

internal sealed class Feed(string title, string newest, string second)
{
    public string Title = title;
    public string Newest = newest;
    public string Second = second;
}
