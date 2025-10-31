using System.Text.Json.Serialization;

namespace BassDJPopup.Settings;

public sealed record AppSettings : IAppSettings
{
    private static IAppSettings _instance;
    public static IAppSettings Instance => _instance ??= new AppSettings();
    public string FolderPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 20;
    public int Height { get; set; } = 20;
    [JsonConverter(typeof(JsonColorConverter))]
    public Color BackColor { get; set; } = Color.MidnightBlue;
    [JsonConverter(typeof(JsonColorConverter))]
    public Color ButtonColor { get; set; } = Color.SteelBlue;
    [JsonConverter(typeof(JsonColorConverter))]
    public Color TriangleColor { get; set; } = Color.LightBlue;

    //public AppSettings Clone() => (AppSettings)MemberwiseClone();
}

