namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    UserControl SettingsControl { get; }
    PluginWidgetBase WidgetControl { get; }
}