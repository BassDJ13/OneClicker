namespace PluginContracts;

public interface IGlobalSettings : IPluginSettings
{
    int WidgetSize { get; set; }
    string HeaderColor { get; set; }
    string BackgroundColor { get; set; }
    string ForegroundColor { get; set; }
}
