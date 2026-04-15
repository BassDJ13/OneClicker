using OneClicker.Classes;
using PluginContracts;

namespace OneClicker.Plugins;

public interface IPluginManager
{
    ActionRegistry? ActionRegistry { get; }
    PluginRegistry? PluginRegistry { get; }
    IList<IPlugin> ActivePlugins { get; }
    IList<IPluginWidgetControl> ActiveWidgets { get; }
    IPlugin GetPlugin(string pluginName);
    IPlugin GetPluginById(string pluginId);
    int WidthOfWidgetsInUnits();
}