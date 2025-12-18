namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    IList<IConfigurationMenuItem> ConfigurationMenuItems { get; }
    IPluginWidgetControl? WidgetInstance { get; }
    IList<IContextMenuItem> ContextMenuItems { get; }

    bool HasConfiguration { get; }
    bool HasWidget { get; }
    bool HasContextMenuItems { get; }

    void Initialize(IPluginSettings settings, IPluginSettings globalSettings);
}