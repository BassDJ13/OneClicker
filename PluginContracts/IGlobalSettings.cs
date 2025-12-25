namespace PluginContracts;

public interface IGlobalSettings : IPluginSettings
{
    int WidgetSize { get; set; }
    Color HeaderColor { get; set; }
    Color BackgroundColor { get; set; }
    Color ForegroundColor { get; set; }
}