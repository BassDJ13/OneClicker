using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : PluginBase
{
    public MainSettingsPlugin()
    {
        Name = "App";
    }

    public override void InitializePlugin()
    {
        Name = "App";
        //AddSettingsItem("General", typeof(MainSettings));
        AddSettingsItem("Appearance", typeof(MainSettings));
        //AddSettingsItem("About", typeof(MainSettings));

        DefaultSettingValues.Add("WindowStyle", nameof(WindowStyle.Floating));
        DefaultSettingValues.Add("DockPosition", nameof(DockPosition.BottomRight));
        DefaultSettingValues.Add("DockOffsetX", "-24");
        DefaultSettingValues.Add("DockOffsetY", "-4");
        DefaultSettingValues.Add("InactiveOpacity", "50");
        DefaultSettingValues.Add("FocusShortcut", "ALT+Z");
    }
}
