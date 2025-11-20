namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    UserControl? SettingsControl { get; }
    PluginWidgetBase? WidgetControl { get; }
    IPluginContextMenu? ContextMenu { get; }

    bool HasSettings { get; }
    bool HasWidget { get; }
    bool HasContextMenu { get; }
}