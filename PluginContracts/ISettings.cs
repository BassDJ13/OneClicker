namespace PluginContracts;

//todo: Make each plugin responsible for it's own settings
public interface ISettings
{
    Color BackColor { get; set; }
    Color ButtonColor { get; set; }
    string FolderPath { get; set; }
    int Height { get; set; }
    Color TriangleColor { get; set; }
    int Width { get; set; }
    int X { get; set; }
    int Y { get; set; }
}