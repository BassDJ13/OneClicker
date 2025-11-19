using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Classes;

internal class PluginManager
{
    private static PluginManager? _instance;
    public static PluginManager Instance
        => _instance ?? throw new InvalidOperationException("PluginManager not initialized.");

    private readonly IMainWindow _mainWindow;

    private PluginManager(IMainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        ActivePlugins = PluginLoader.LoadPlugins("plugins", mainWindow);

        IList<string> names = new List<string>();
        foreach (var plugin in ActivePlugins)
        {
            names.Add(plugin.Name);
        }
        Names = names.ToArray();
    }

    public static void Initialize(IMainWindow mainWindow)
    {
        if (_instance != null)
            throw new InvalidOperationException("PluginManager already initialized.");

        _instance = new PluginManager(mainWindow);
    }

    public IList<IPlugin> ActivePlugins;
    public string[] Names { get; private set; }

    public IPlugin GetPlugin(string pluginName)
    {
        foreach (var plugin in ActivePlugins)
        {
            if (plugin.Name == pluginName)
            {
                return plugin;
            }
        }
        throw new KeyNotFoundException();
    }
}
