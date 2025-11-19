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
    public int Width { get; set; } = 20;
    public int Height { get; set; } = 20;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color BackColor { get; set; } = Color.MidnightBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color ButtonColor { get; set; } = Color.SteelBlue;
    //[JsonConverter(typeof(JsonColorConverter))]
    public Color TriangleColor { get; set; } = Color.LightBlue;

    public ISettings Copy()
    {
        return new Settings()
        {
            FolderPath = this.FolderPath,
            X = this.X,
            Y = this.Y,
            Width = this.Width,
            Height = this.Height,
            BackColor = this.BackColor,
            ButtonColor = this.ButtonColor,
            TriangleColor = this.TriangleColor
        };
    }

    public void Save(ISettings settings)
    {
        FolderPath = settings.FolderPath;
        X = settings.X;
        Y = settings.Y;
        Width = settings.Width;
        Height = settings.Height;
        BackColor = settings.BackColor;
        ButtonColor = settings.ButtonColor;
        TriangleColor = settings.TriangleColor;
    }
}

