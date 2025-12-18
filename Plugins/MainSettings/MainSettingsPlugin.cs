using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : Plugin
{
    public override string Name => "App";

    protected override Type? WidgetClass => null;

    protected override void InitializeConfigurationControl()
    {
        //AddSettingsItem("General", typeof(MainSettings));
        AddConfigurationControl("Appearance", typeof(MainSettingsConfiguration));
        //AddSettingsItem("About", typeof(MainSettings));
    }

    protected override void InitializePluginSettings()
    {
        DefaultSettingValues.Add("WindowStyle", nameof(WindowStyle.Floating));
        DefaultSettingValues.Add("DockPosition", nameof(DockPosition.BottomRight));
        DefaultSettingValues.Add("DockOffsetX", "-24");
        DefaultSettingValues.Add("DockOffsetY", "-4");
        DefaultSettingValues.Add("InactiveOpacity", "50");
        DefaultSettingValues.Add("FocusShortcut", "ALT+Z");
    }
}
