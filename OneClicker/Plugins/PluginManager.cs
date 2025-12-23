using OneClicker.Classes;
using PluginContracts;

namespace OneClicker.Plugins;

internal class PluginManager
{
    private static PluginManager? _instance;
    public static PluginManager Instance
        => _instance ?? throw new InvalidOperationException("PluginManager not initialized.");

    public ActionRegistry? ActionRegistry { get; private set; }

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

    private IList<IPlugin>? GetPluginsWithWidgets(IList<IPlugin> plugins)
    {
        var result = new List<IPlugin>();
        foreach (var plugin in plugins)
        {
            if (plugin.HasWidget)
            {
                result.Add(plugin);
            }
        }
        return result;
    }

    private IReadOnlyList<PluginActionDescriptor>? GetAllPluginActions(IList<IPlugin> plugins)
    {
        if (ActionRegistry != null)
        {
            return ActionRegistry.GetAllActions();
        }

        var result = new List<PluginActionDescriptor>();
        foreach (var plugin in plugins)
        {
            int i = 1;
            foreach (var action in plugin.Actions)
            {
                result.Add(new PluginActionDescriptor(
                    pluginId: plugin.Guid.ToString(),
                    actionId: action.Key,
                    displayName: $"{plugin.Name}.{action.Key}"));
                i++;
            }
        }
        ActionRegistry = new ActionRegistry(result);
        SupplyAllPluginActionsToPlugins();
        return result;
    }

    private void SupplyAllPluginActionsToPlugins()
    {
        var actions = new List<PluginActionDescriptor>();

        foreach (var plugin in ActivePlugins)
        {
            if (plugin is IRequiresActionRegistry pluginWithActionRegistry)
            {
                pluginWithActionRegistry.InitializeActions(ActionRegistry!);
            }
        }
    }

    public static void Initialize()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("PluginManager already initialized.");
        }

        _instance = new PluginManager();
    }

    private IList<IPlugin> _activePlugins;
    public IList<IPlugin> ActivePlugins
        => _activePlugins;

    public IList<IPlugin> ActiveWidgets
        => GetPluginsWithWidgets(ActivePlugins)!;

    public IReadOnlyList<PluginActionDescriptor> ActiveActions
        => GetAllPluginActions(ActivePlugins)!;

    public string[] Names { get; private set; }

    public IPlugin GetPlugin(string pluginName)
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
}
