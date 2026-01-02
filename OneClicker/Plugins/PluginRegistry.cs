using PluginContracts;

namespace OneClicker.Plugins;

internal sealed class PluginRegistry : IPluginRegistry
{
    private readonly IList<IPlugin> _plugins;

    public PluginRegistry(IList<IPlugin> plugins)
    {
        _plugins = plugins;
    }

    public IList<IPlugin> GetAllPlugins() => _plugins;

    public IPlugin? GetPlugin(string id)
    {
        foreach (var p in _plugins)
        {
            if (p.Guid.ToString() == id) return p;
        }
        return null;
    }

    //public IPlugin? GetPluginByName(string name)
    //{
    //    foreach (var p in _plugins)
    //    {
    //        if (p.Name == name) return p;
    //    }
    //    return null;
    //}
}
