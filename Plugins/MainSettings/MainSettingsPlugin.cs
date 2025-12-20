using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : Plugin
{
    public override string Name => "App";

    protected override void InitializeConfigurationControls()
    {
        //AddConfigurationControl("General", typeof(MainSettingsConfiguration));
        AddConfigurationControl("Appearance", typeof(MainSettingsConfiguration));
        //AddConfigurationControl("Plugins", typeof(MainSettingsConfiguration));
        //AddConfigurationControl("About", typeof(MainSettingsConfiguration));
    }

    protected override void InitializePluginSettings()
    {
        AddSetting("WindowStyle", nameof(WindowStyle.Floating));
        AddSetting("DockPosition", nameof(DockPosition.BottomRight));
        AddSetting("DockOffsetX", "-24");
        AddSetting("DockOffsetY", "-4");
        AddSetting("InactiveOpacity", "50");
        AddSetting("FocusShortcut", "ALT+Z");
    }
}
