namespace MyRss;

public static class StringExtensions
{
    private static Random rng = new();
    public static ColoredString CreateRandomColored(this string str)
    {
        var r = rng.Next(140, 230);
        var g = rng.Next(140, 230);
        var b = rng.Next(140, 230);
        var background = new Color(r, g, b);
        return new ColoredString(" " + str + " ", Color.Black, background);
    }
}