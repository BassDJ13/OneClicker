using MainSettings.Controls;
using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsPlugin : Plugin, IRequiresActionRegistry
{
    public override Guid Guid => new Guid("FB7AD8BB-1BAF-4D55-9DDA-20D78B4CC72C");

    public override string Name => "App";

    private IActionRegistry? _allPluginsActions;
    public void InitializeActions(IActionRegistry registry)
    {
        _allPluginsActions = registry;
    }

    protected override void InitializeConfigurationControls()
    {
        AddConfigurationControl("General", typeof(GeneralSettings)); //todo: add third parameter for extra constructor parameters
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
