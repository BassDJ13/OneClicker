namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    IList<ISettingsItem> SettingsItems { get; }
    IPluginWidgetControlBase? WidgetInstance { get; }
    IList<MenuItem> ContextMenuItems { get; }

    bool HasSettings { get; }
    bool HasWidget { get; }
    bool HasMenuItems { get; }

    void Initialize(IPluginSettings settings, IPluginSettings globalSettings);
}