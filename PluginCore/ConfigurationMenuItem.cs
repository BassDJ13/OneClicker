using PluginContracts;

namespace PluginCore;

public sealed class ConfigurationMenuItem : IConfigurationMenuItem 
{
    public IPluginSettings PluginSettings { get; private set; }
    public IPluginSettings GlobalSettings { get; private set; }
    public string Name { get; }
    public Type? ConfigurationClass { get; }
    public string PluginId { get; private set; }
    private IPluginConfigurationControl? _content;
    public IPluginConfigurationControl? Content => _content;

    public IPluginConfigurationControl? CreateContent(IPluginSettings pluginSettingsOverlay, IPluginSettings globalSettingsOverlay)
    {
        if (ConfigurationClass == null)
        {
            return null;
        }

        if (_content != null && !((PluginConfigurationControl)_content).IsDisposed)
        {
            return _content;
        }

        _content = (IPluginConfigurationControl)Activator.CreateInstance(ConfigurationClass, pluginSettingsOverlay, globalSettingsOverlay)!;

        return _content;
    }

    public ConfigurationMenuItem(string name, Type? configurationClass, string pluginId, IPluginSettings pluginSettings, IPluginSettings globalSettings)
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
        PluginId = pluginId;
        Name = name;
        ConfigurationClass = configurationClass;
    }
}