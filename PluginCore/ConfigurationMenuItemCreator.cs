using PluginContracts;

namespace PluginCore;

internal class ConfigurationMenuItemCreator
{
    private string _pluginId;

    public ConfigurationMenuItemCreator(string pluginId)
    {
        _pluginId = pluginId;
    }

    public IConfigurationMenuItem Create(string name, Type? configurationClass)
    {
        return new ConfigurationMenuItem(name, configurationClass, _pluginId);
    }
}