using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : PluginBase
{
    public MainSettingsPlugin()
    {
        Name = "Appearance";
        //SettingsItems.Add(new SettingsItem("General", typeof(MainSettings)));
        SettingsItems.Add(new SettingsItem("Appearance", typeof(MainSettings)));
        //SettingsItems.Add(new SettingsItem("About", typeof(MainSettings)));
    }
}
