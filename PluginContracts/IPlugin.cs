namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    IList<ISettingsItem> SettingsItems { get; }
    IPluginWidgetBase? WidgetControl { get; }
    IList<MenuItem> MenuItems { get; }

    bool HasSettings { get; }
    bool HasWidget { get; }
    bool HasMenuItems { get; }
}