using PluginContracts;
using PluginCore;

namespace MainSettings;

public class PluginsSettings : PluginConfigurationControl
{
    public PluginsSettings(IPluginContext pluginContext) : base(pluginContext)
    {
        var plugins = ((IHostPluginContext)pluginContext)
            .PluginRegistry
            .GetAllPlugins();

        var label = new Label
        {
            Text = "Enabled plugins:",
            Left = 0,
            Top = 0,
            AutoSize = true
        };

        var pluginList = new CheckedListBox
        {
            Left = 0,
            Top = label.Bottom,
            Width = 300,
            Height = 175,
            CheckOnClick = false,
            Enabled = false
        };

        foreach (IPlugin plugin in plugins)
        {
            pluginList.Items.Add(plugin.Name, true);
        }

        pluginList.ItemCheck += (s, e) =>
        {
            e.NewValue = e.CurrentValue;
        };

        Controls.Add(label);
        Controls.Add(pluginList);
    }
}
