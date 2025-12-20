namespace PluginContracts;

public static class PluginSettingsExtensions
{
    public static int GetInt(this IPluginSettings s, string key, int defaultValue = 0)
        => int.TryParse(s.Get(key), out var v) ? v : defaultValue;

    public static void SetInt(this IPluginSettings s, string key, int value)
        => s.Set(key, value.ToString());

    public static bool GetBool(this IPluginSettings s, string key, bool defaultValue = false)
        => bool.TryParse(s.Get(key), out var v) ? v : defaultValue;

    public static void SetBool(this IPluginSettings s, string key, bool value)
        => s.Set(key, value.ToString());

    public static double GetDouble(this IPluginSettings s, string key, double defaultValue = 0f)
        => double.TryParse(s.Get(key), out var v) ? v : defaultValue;

    public static void SetDouble(this IPluginSettings s, string key, double value)
        => s.Set(key, value.ToString());

    public static Color GetColor(this IPluginSettings s, string key, Color? defaultValue = null)
    {
        var fallback = defaultValue ?? Color.White;
        return HexToColor(s.Get(key)!, fallback);
    }

    public static void SetColor(this IPluginSettings s, string key, Color value)
        => s.Set(key, ColorToHex(value));


    private static Color HexToColor(string hex, Color fallback)
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

    private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}