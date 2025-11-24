//using OneClicker.Settings.Json;
using System.Text.Json.Serialization;

namespace PluginContracts;

public sealed record AppSettings : IAppSettings
{
    private static IAppSettings? _instance;
    public static IAppSettings Instance => _instance ??= new AppSettings();
    public string FolderPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    public int X { get; set; }
    public int Y { get; set; }
    public int WidgetSize { get; set; } = 16;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color BackColor { get; set; } = Color.MidnightBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color ButtonColor { get; set; } = Color.SteelBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color TriangleColor { get; set; } = Color.LightBlue;
    public WindowStyle WindowStyle { get; set; } = WindowStyle.Floating;
    public DockPosition DockPosition { get; set; } = DockPosition.BottomRight;
    public int DockOffsetX { get; set; }
    public int DockOffsetY { get; set; }
    public int InactiveOpacity { get; set; } = 50;
    public string FocusShortcut { get; set; } = "ALT+Z";

    public ISettings Copy()
    {
        return new Settings()
        {
            FolderPath = this.FolderPath,
            X = this.X,
            Y = this.Y,
            WidgetSize = this.WidgetSize,
            BackColor = this.BackColor,
            ButtonColor = this.ButtonColor,
            TriangleColor = this.TriangleColor,
            WindowStyle = this.WindowStyle,
            DockPosition = this.DockPosition,
            DockOffsetX = this.DockOffsetX,
            DockOffsetY = this.DockOffsetY,
            InactiveOpacity = this.InactiveOpacity,
            FocusShortcut = this.FocusShortcut
        };
    }

    public void Save(ISettings settings)
    {
        FolderPath = settings.FolderPath;
        X = settings.X;
        Y = settings.Y;
        WidgetSize = settings.WidgetSize;
        BackColor = settings.BackColor;
        ButtonColor = settings.ButtonColor;
        TriangleColor = settings.TriangleColor;
        WindowStyle = settings.WindowStyle;
        DockPosition = settings.DockPosition;
        DockOffsetX = settings.DockOffsetX;
        DockOffsetY = settings.DockOffsetY;
        InactiveOpacity = settings.InactiveOpacity;
        FocusShortcut = settings.FocusShortcut;
    }
}

