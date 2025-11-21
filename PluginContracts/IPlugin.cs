namespace PluginContracts;

public interface IPlugin
{
    string Name { get; }

    UserControl? SettingsControl { get; }
    PluginWidgetBase? WidgetControl { get; }
    IList<MenuItem> MenuItems { get; }

    bool HasSettings { get; }
    bool HasWidget { get; }
    bool HasMenuItems { get; }
}