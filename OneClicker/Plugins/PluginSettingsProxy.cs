using PluginContracts;

namespace OneClicker.Plugins;

public class PluginSettingsProxy : IPluginSettings
{
    private readonly string _prefix;
    private readonly ISettingsStore _store;

    public PluginSettingsProxy(string pluginName, ISettingsStore store)
    {
        _prefix = pluginName + ".";
        _store = store;
    }

    public string Get(string key)
        => _store.Get(_prefix + key)!;

    public void Set(string key, string value)
        => _store.Set(_prefix + key, value);
}