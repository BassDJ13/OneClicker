using PluginContracts;

namespace PluginCore;

public abstract class PluginSettingsControlBase : UserControl, IPluginSettingsControlBase
{
    public IPluginSettings PluginSettings { get; private set; }
    public IPluginSettings GlobalSettings { get; private set; }

    public PluginSettingsControlBase(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base()
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
    }
}
