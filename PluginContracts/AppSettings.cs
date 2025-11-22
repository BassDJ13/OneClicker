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
    public int WidgetSize { get; set; } = 20;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color BackColor { get; set; } = Color.MidnightBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color ButtonColor { get; set; } = Color.SteelBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color TriangleColor { get; set; } = Color.LightBlue;
    public WindowStyle WindowStyle { get; set; } = WindowStyle.Floating;
    public DockPosition DockPosition { get; set; } = DockPosition.BottomRight;
    public int InactiveOpacity { get; set; } = 70;

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
            InactiveOpacity = this.InactiveOpacity
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
        InactiveOpacity = settings.InactiveOpacity;
    }
}

