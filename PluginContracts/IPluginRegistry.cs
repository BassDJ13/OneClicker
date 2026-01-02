namespace PluginContracts;

public interface IPluginRegistry
{
    IList<IPlugin> GetAllPlugins();
    IPlugin? GetPlugin(string pluginId);
}