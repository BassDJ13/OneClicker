using BassCommon;
using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Settings;

internal static class GlobalSettings
{
    internal static PluginSettingsProxy Initialize(ISettingsStore settingsStore)
    {
        var globalSettings = new PluginSettingsProxy("Global", settingsStore);

        SetDefaultGlobalSettings(globalSettings);

        foreach (var plugin in PluginManager.Instance.ActivePlugins)
        {
            var settingsProxy = new PluginSettingsProxy(plugin.Name, settingsStore);
            plugin.Initialize(settingsProxy, globalSettings);
        }

        return globalSettings;
    }

    private static void SetDefaultGlobalSettings(PluginSettingsProxy globalSettings)
    {
        var _defaultSettingValues = new Dictionary<string, string>
        {
            { "WidgetSize", "16" },
            { GlobalSettingKeys.HeaderColor, ColorHelper.ColorToHex(Color.MidnightBlue) },
            { GlobalSettingKeys.BackgroundColor, ColorHelper.ColorToHex(Color.SteelBlue) },
            { GlobalSettingKeys.ForegroundColor, ColorHelper.ColorToHex(Color.LightBlue) }
        };

        foreach (var kvp in _defaultSettingValues)
        {
            if (globalSettings!.Get(kvp.Key) == null)
            {
                globalSettings.Set(kvp.Key, kvp.Value);
            }
        }
    }
}
