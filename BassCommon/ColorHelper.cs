namespace BassCommon;

public static class ColorHelper
{
    public static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static Color HexToColor(string hex, Color fallback)
    {
        if (hex == null)
        {
            return fallback;
        }

        try
        {
            return ColorTranslator.FromHtml(hex);
        }
        catch
        {
            return fallback;
        }
    }

    /// <summary>
    /// Takes two colors and returns a color between them
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <param name="between">0 returns color1, 1 return color2, between 0 and 1 returns a color between color1 and color2</param>
    /// <returns></returns>
    public static Color GetColorBetween(Color color1, Color color2, double between)
    {
        between = Math.Clamp(between, 0, 1);
        int r = (int)(color1.R + (color2.R - color1.R) * between);
        int g = (int)(color1.G + (color2.G - color1.G) * between);
        int b = (int)(color1.B + (color2.B - color1.B) * between);
        return Color.FromArgb(r, g, b);
    }


}
