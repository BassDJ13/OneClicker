using PluginContracts;

namespace OneClicker.Settings;

public interface IMainAppSettings : IPluginSettings
{
    double InactiveOpacity { get; set; }
    WindowStyle WindowStyle { get; set; }
    DockPosition DockPosition { get; set; }
    int DockOffsetX { get; set; }
    int DockOffsetY { get; set; }
    int X { get; set; }
    int Y { get; set; }
    string FocusShortcut { get; set; }
    string ShortcutAction { get; set; }
}