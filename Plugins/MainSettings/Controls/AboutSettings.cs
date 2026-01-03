using PluginContracts;
using PluginCore;
using System.Diagnostics;

namespace MainSettings;

public class AboutSettings : PluginConfigurationControl
{
    public AboutSettings(IPluginContext pluginContext) : base(pluginContext)
    {
        var textbox = new TextBox
        {
            Left = 0,
            Top = 0,
            Multiline = true,
            Width = 320,
            Height = 185,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Text = "This app was created by Bastiaan de Jong." + Environment.NewLine
                + "" + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
        };
        var label = new Label { Text = "Website:", Top = 190, Left = 0, AutoSize = true };
        var link = new LinkLabel { Text = "https://github.com/BassDJ13/OneClicker", Top = 190, Left = 50, AutoSize = true };
        link.LinkClicked += Link_Clicked;
        Controls.AddRange([textbox, label, link]);
    }

    private void Link_Clicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/BassDJ13/OneClicker")
        {
            UseShellExecute = true
        });
    }
}
