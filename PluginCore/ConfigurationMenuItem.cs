using PluginContracts;

namespace PluginCore;

public sealed class ConfigurationMenuItem : IConfigurationMenuItem 
{
    public string Name { get; }
    public Type? ConfigurationClass { get; }
    public string PluginId { get; private set; }
    private IPluginConfigurationControl? _content;
    public IPluginConfigurationControl? Content => _content;

    public IPluginConfigurationControl? CreateConfigurationControl(IPluginContext context)
    {
        if (ConfigurationClass == null)
        {
            return null;
        }

        if (_content != null && !((PluginConfigurationControl)_content).IsDisposed)
        {
            return _content;
        }

        _content = (IPluginConfigurationControl)Activator.CreateInstance(ConfigurationClass, [context])!;

        return _content;
    }

    public ConfigurationMenuItem(string name, Type? configurationClass, string pluginId)
    {
        PluginId = pluginId;
        Name = name;
        ConfigurationClass = configurationClass;
    }
}