namespace PluginContracts;

//todo: becomes obsolete after settings refactor
public interface ISettings
{
    Color BackColor { get; set; }
    Color ButtonColor { get; set; }
    string FolderPath { get; set; }
    Color TriangleColor { get; set; }
    int WidgetSize { get; set; }
    int X { get; set; }
    int Y { get; set; }
    WindowStyle WindowStyle { get; set; }
    DockPosition DockPosition { get; set; }
    int DockOffsetX { get; set; }
    int DockOffsetY { get; set; }
    int InactiveOpacity { get; set; }
    string FocusShortcut { get; set; }
}