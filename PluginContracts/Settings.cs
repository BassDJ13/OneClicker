namespace PluginContracts;

public sealed record Settings : ISettings
{
    public string FolderPath { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int WidgetSize { get; set; }
    public Color BackColor { get; set; }
    public Color ButtonColor { get; set; }
    public Color TriangleColor { get; set; }
    public WindowStyle WindowStyle { get; set; }
    public DockPosition DockPosition { get; set; }
    public int DockOffsetX { get; set; }
    public int DockOffsetY { get; set; }
    public int InactiveOpacity { get; set; }
}

