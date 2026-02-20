
using System.Windows.Media;
using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Settings;

internal class GlobalSettings : PluginSettingsProxy, IGlobalSettings
{
    private const string _prefix = "Global";
    private const string _widgetSize = "WidgetSize";
    private const string _headerColor = "HeaderColor";
    private const string _backgroundColor = "BackgroundColor";
    private const string _foregroundColor = "ForegroundColor";

    public int WidgetSize
    {
        get => this.GetInt(_widgetSize, 16);
        set => this.SetInt(_widgetSize, value);
    }

    public string HeaderColor
    {
        get => Get(_headerColor) ?? "#FF191970"; // MidnightBlue
        set => Set(_headerColor, value);
    }

    public string BackgroundColor
    {
        get => Get(_backgroundColor) ?? "#FF4682B4"; // SteelBlue
        set => Set(_backgroundColor, value);
    }

    public string ForegroundColor
    {
        get => Get(_foregroundColor) ?? "#FFADD8E6"; // LightBlue
        set => Set(_foregroundColor, value);
    }

    internal GlobalSettings(ISettingsStore store) : base(_prefix, store)
    {
    }
}
