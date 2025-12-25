using PluginContracts;

namespace PluginCore;

public abstract class PluginConfigurationControl : UserControl, IPluginConfigurationControl
{
    public IPluginSettings PluginSettings { get; private set; }
    public IGlobalSettings GlobalSettings { get; private set; }

    public PluginConfigurationControl(IPluginSettings pluginSettings, IGlobalSettings globalSettings) : base()
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
    }
}
