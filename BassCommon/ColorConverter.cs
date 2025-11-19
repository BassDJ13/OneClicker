namespace BassCommon;

public static class ColorConverter
{
    public static Color Convert(Color from, Color to, double t)
    {
        t = Math.Clamp(t, 0, 1);
        int r = (int)(from.R + (to.R - from.R) * t);
        int g = (int)(from.G + (to.G - from.G) * t);
        int b = (int)(from.B + (to.B - from.B) * t);
        return Color.FromArgb(r, g, b);
    }
}
