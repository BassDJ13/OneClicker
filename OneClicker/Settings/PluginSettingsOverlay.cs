using PluginContracts;

namespace OneClicker.Settings;

public sealed class PluginSettingsOverlay : SettingsOverlayBase
{
    public PluginSettingsOverlay(string pluginId, ISettingsStore store)
    : base(store, pluginId + ".")
    {
    }
}
