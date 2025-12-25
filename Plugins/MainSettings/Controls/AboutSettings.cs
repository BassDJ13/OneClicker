using PluginContracts;
using PluginCore;

namespace MainSettings;

public class AboutSettings : PluginConfigurationControl
{
    public AboutSettings(IPluginSettings pluginSettings, IGlobalSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelStyle = new Label { Text = "Bastiaan de Jong", Left = 0, Top = 0, Width = 100 };

        Controls.AddRange([
            labelStyle]);
    }
}
