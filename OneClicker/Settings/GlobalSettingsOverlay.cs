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

    public Color HeaderColor
    {
        get => this.GetColor(_headerColor, Color.MidnightBlue);
        set => this.SetColor(_headerColor, value);
    }

    public Color BackgroundColor
    {
        get => this.GetColor(_backgroundColor, Color.SteelBlue);
        set => this.SetColor(_backgroundColor, value);
    }

    public Color ForegroundColor
    {
        get => this.GetColor(_foregroundColor, Color.LightBlue);
        set => this.SetColor(_foregroundColor, value);
    }

    public GlobalSettingsOverlay(ISettingsStore store)
        : base(store, "Global.")
    {
    }
}
