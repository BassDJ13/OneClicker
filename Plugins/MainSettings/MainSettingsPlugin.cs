using MainSettings.Controls;
using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : Plugin
{
    public override string Name => "App";

    protected override void InitializeConfigurationControls()
    {
        AddConfigurationControl("General", typeof(GeneralSettings));
        AddConfigurationControl("Appearance", typeof(AppearanceSettings));
        AddConfigurationControl("Plugins", typeof(PluginsSettings));
        AddConfigurationControl("About", typeof(AboutSettings));
    }

    protected override void InitializeSettings()
    {
        AddSetting(SettingKeys.WindowStyle, nameof(WindowStyle.Floating));
        AddSetting(SettingKeys.DockPosition, nameof(DockPosition.BottomRight));
        AddSetting(SettingKeys.DockOffsetX, "-24");
        AddSetting(SettingKeys.DockOffsetY, "-4");
        AddSetting(SettingKeys.InactiveOpacity, "50");
        AddSetting(SettingKeys.FocusShortcut, "ALT+Z");
    }
}
