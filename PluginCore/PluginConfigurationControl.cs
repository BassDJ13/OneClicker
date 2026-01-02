using PluginContracts;

namespace PluginCore;

public abstract class PluginConfigurationControl : UserControl, IPluginConfigurationControl
{
    protected IPluginSettings PluginSettings { get; private set; }
    protected IGlobalSettings GlobalSettings { get; private set; }

    public PluginConfigurationControl(IPluginContext pluginContext) : base()
    {
        PluginSettings = pluginContext.PluginSettings;
        GlobalSettings = pluginContext.GlobalSettings;
    }
}
