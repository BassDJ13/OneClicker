using PluginContracts;

namespace OneClicker.Settings;

public sealed class GlobalSettingsOverlay : SettingsOverlayBase, IGlobalSettings
{
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
        get => Get(_headerColor) ?? "#FF191970";
        set => Set(_headerColor, value);
    }

    public string BackgroundColor
    {
        get => Get(_backgroundColor) ?? "#FF4682B4";
        set => Set(_backgroundColor, value);
    }

    public string ForegroundColor
    {
        get => Get(_foregroundColor) ?? "#FFADD8E6";
        set => Set(_foregroundColor, value);
    }

    public GlobalSettingsOverlay(ISettingsStore store)
        : base(store, "Global")
    {
    }
}
