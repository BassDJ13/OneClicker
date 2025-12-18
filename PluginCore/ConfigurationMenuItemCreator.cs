using PluginContracts;

namespace PluginCore;

internal class ConfigurationMenuItemCreator
{
    public IPluginSettings PluginSettings { get; }
    public IPluginSettings GlobalSettings { get; }

    private string _pluginId;

    public ConfigurationMenuItemCreator(string pluginId, IPluginSettings pluginSettings, IPluginSettings globalSettings)
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
        _pluginId = pluginId;
    }

    public IConfigurationMenuItem Create(string name, Type? configurationClass)
    {
        return new ConfigurationMenuItem(name, configurationClass, _pluginId, PluginSettings, GlobalSettings);
    }
}