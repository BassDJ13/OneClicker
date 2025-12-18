using PluginContracts;

namespace OneClicker.Plugins;

internal class PluginManager
{
    private static PluginManager? _instance;
    public static PluginManager Instance
        => _instance ?? throw new InvalidOperationException("PluginManager not initialized.");

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
}
