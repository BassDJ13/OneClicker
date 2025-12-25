using PluginContracts;
using PluginCore;

namespace MainSettings;

public class PluginsSettings : PluginConfigurationControl
{
    public PluginsSettings(IPluginSettings pluginSettings, IGlobalSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelStyle = new Label { Text = "Plugins settings", Left = 0, Top = 0, Width = 100 };

        Controls.AddRange([
            labelStyle]);
    }
}
