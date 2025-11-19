namespace PluginContracts;

public sealed record Settings : ISettings
{
    public string FolderPath { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 20;
    public int Height { get; set; } = 20;
    public Color BackColor { get; set; } = Color.MidnightBlue;
    public Color ButtonColor { get; set; } = Color.SteelBlue;
    public Color TriangleColor { get; set; } = Color.LightBlue;
}

