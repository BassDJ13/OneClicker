namespace PluginContracts;

public interface IPlugin
{
    Guid Guid { get; }
    string Name { get; }
    IList<IConfigurationMenuItem> ConfigurationMenuItems { get; }
    IPluginWidgetControl? WidgetInstance { get; }
    IList<IContextMenuItem> ContextMenuItems { get; }
    bool HasConfiguration { get; }
    bool HasWidget { get; }
    bool HasContextMenuItems { get; }
    Dictionary<string, Action> Actions { get; }

    void PreInitialize(IPluginContext context);
    void PostInitialize();
}