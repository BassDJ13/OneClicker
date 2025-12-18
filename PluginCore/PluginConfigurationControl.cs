using PluginContracts;

namespace PluginCore;

public abstract class PluginConfigurationControl : UserControl, IPluginConfigurationControl
{
    public IPluginSettings PluginSettings { get; private set; }
    public IPluginSettings GlobalSettings { get; private set; }

    public PluginConfigurationControl(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base()
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
    }
}
