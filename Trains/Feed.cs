namespace MyRss;
internal sealed class Feed(string title, string newest, string second)
{
    public readonly string Title = title;
    public readonly string Newest = newest;
    public readonly string Second = second;
}