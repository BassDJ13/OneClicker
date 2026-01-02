using OneClicker.Classes;
using PluginContracts;

namespace OneClicker.Plugins;

public class PluginManager
{
    internal ActionRegistry? ActionRegistry { get; private set; }
    internal PluginRegistry? PluginRegistry { get; private set; }

    public PluginManager(ISettingsStore settingsStore, IGlobalSettings globalSettings)
    {
        _activePlugins = PluginLoader.LoadPlugins("plugins");
        PluginRegistry = new PluginRegistry(_activePlugins);

        foreach (var plugin in ActivePlugins)
        {
            var settingsProxy = new PluginSettingsProxy(plugin.Name, settingsStore);
            var context = CreateWidgetContext(settingsProxy, globalSettings);
            plugin.PreInitialize(context);
        }

        ActionRegistry = new ActionRegistry(_activePlugins);

        foreach (var plugin in ActivePlugins)
        {
            plugin.PostInitialize();
        }
    }

    private IList<IPluginWidgetControl>? GetPluginsWithWidgets(IList<IPlugin> plugins)
    {
        var result = new List<IPluginWidgetControl>();
        foreach (var plugin in plugins)
        {
            if (plugin.HasWidget)
            {
                result.Add(plugin.WidgetInstance!);
            }
        }
        return result;
    }

    private IList<IPlugin> _activePlugins;
    internal IList<IPlugin> ActivePlugins
        => _activePlugins;

    internal IList<IPluginWidgetControl> ActiveWidgets
        => GetPluginsWithWidgets(ActivePlugins)!;

    internal IPlugin GetPlugin(string pluginName)
    {
        foreach (var plugin in ActivePlugins!)
        {
            if (plugin.Name == pluginName)
            {
                return plugin;
            }
        }
        throw new KeyNotFoundException();
    }

    internal IPlugin GetPluginById(string pluginId)
    {
        foreach (var plugin in ActivePlugins!)
        {
            if (plugin.Guid.ToString() == pluginId)
            {
                return plugin;
            }
        }
        throw new KeyNotFoundException();
    }

    private IPluginContext CreateWidgetContext(PluginSettingsProxy pluginSettings, IGlobalSettings globalSettings)
        => new PluginContext(
            pluginSettings: pluginSettings,
            globalSettings: globalSettings,
            actionRegistry: ActionRegistry!,
            pluginRegistry: PluginRegistry!);

    internal int WidthOfWidgetsInUnits()
    {
        int width = 0;
        foreach (IPluginWidgetControl widget in ActiveWidgets)
        {
            width += widget.WidthInUnits;
        }
        return width;
    }
}
