using PluginContracts;

namespace PluginCore;

public sealed class ConfigurationMenuItem : IConfigurationMenuItem 
{
    public string Name { get; }
    public Type? ConfigurationClass { get; }
    public string PluginId { get; private set; }
    private IPluginConfigurationControl? _content;
    public IPluginConfigurationControl? Content => _content;
    public object[] _customParameters;

    public IPluginConfigurationControl? CreateConfigurationControl(IPluginSettings pluginSettingsOverlay, IGlobalSettings globalSettingsOverlay)
    {
        if (ConfigurationClass == null)
        {
            return null;
        }

        if (_content != null && !((PluginConfigurationControl)_content).IsDisposed)
        {
            return _content;
        }

        var parameters = new object[]
        {
            pluginSettingsOverlay,
            globalSettingsOverlay
        }
        .Concat(_customParameters)
        .ToArray();

        _content = (IPluginConfigurationControl)Activator.CreateInstance(ConfigurationClass, parameters)!;

        return _content;
    }

    public ConfigurationMenuItem(string name, Type? configurationClass, string pluginId, params object[] customParameters)
    {
        PluginId = pluginId;
        Name = name;
        ConfigurationClass = configurationClass;
        _customParameters = customParameters;
    }
}