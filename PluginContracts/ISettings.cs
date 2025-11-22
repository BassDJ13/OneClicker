namespace PluginContracts;

//todo: Make each plugin responsible for it's own settings
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
    int InactiveOpacity { get; set; }
}