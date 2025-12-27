using OneClicker.Classes;
using PluginContracts;

namespace OneClicker.Plugins;

internal class PluginManager
{
    private static PluginManager? _instance;
    internal static PluginManager Instance => _instance ??= new PluginManager();

    internal ActionRegistry? ActionRegistry { get; private set; }

    private PluginManager()
    {
        _activePlugins = PluginLoader.LoadPlugins("plugins");
        IList<string> names = new List<string>();
        foreach (var plugin in ActivePlugins)
        {
            names.Add(plugin.Name);
        }
        Names = names.ToArray();
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

    private IList<PluginActionDescriptor> GetAllPluginActions(IList<IPlugin> plugins)
    {
        if (ActionRegistry != null)
        {
            return ActionRegistry.GetAllActions();
        }

        var result = new List<PluginActionDescriptor>();
        foreach (var plugin in plugins)
        {
            foreach (var action in plugin.Actions)
            {
                result.Add(new PluginActionDescriptor(
                    pluginId: plugin.Guid.ToString(),
                    actionId: action.Key,
                    pluginName: plugin.Name));
            }
        }
        
        return result;
    }

    internal void SupplyAllPluginActionsToPlugins()
    {
        ActionRegistry = new ActionRegistry(GetAllPluginActions(ActivePlugins));
        foreach (var plugin in ActivePlugins)
        {
            if (plugin is IRequiresActionRegistry pluginWithActionRegistry)
            {
                pluginWithActionRegistry.SupplyActions(ActionRegistry!);
            }
        }
    }

    private IList<IPlugin> _activePlugins;
    internal IList<IPlugin> ActivePlugins
        => _activePlugins;

    internal IList<IPluginWidgetControl> ActiveWidgets
        => GetPluginsWithWidgets(ActivePlugins)!;

    internal string[] Names { get; private set; }

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

    internal void InitializePlugins(ISettingsStore settingsStore, IGlobalSettings globalSettings)
    {
        foreach (var plugin in ActivePlugins)
        {
            var settingsProxy = new PluginSettingsProxy(plugin.Name, settingsStore);
            plugin.PreInitialize(settingsProxy, globalSettings);
        }
        SupplyAllPluginActionsToPlugins();
        foreach (var plugin in ActivePlugins)
        {
            plugin.PostInitialize();
        }
    }

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
