using PluginContracts;

namespace OneClicker.Settings;

public sealed class GlobalSettingsOverlay : SettingsOverlayBase
{
    public GlobalSettingsOverlay(ISettingsStore store)
        : base(store, "Global.")
    {
    }
}
