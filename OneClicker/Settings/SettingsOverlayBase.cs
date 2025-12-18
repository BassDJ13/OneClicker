using PluginContracts;

namespace OneClicker.Settings;

public abstract class SettingsOverlayBase : IPluginSettings
{
    private ISettingsStore _store;
    private string _prefix;

    private readonly Dictionary<string, string?> _buffer = new();

    public SettingsOverlayBase(ISettingsStore store, string prefix)
    {
        _store = store;
        _prefix = prefix;
    }

    public string? Get(string key)
    {
        var fullKey = _prefix + key;
        return _buffer!.TryGetValue(fullKey, out var value)
            ? value
            : _store!.Get(fullKey);
    }

    public void Set(string key, string? value)
    {
        _buffer[_prefix + key] = value;
    }

    public void Commit()
    {
        foreach (var kv in _buffer)
        {
            _store.Set(kv.Key, kv.Value!);
        }
        _buffer.Clear();
    }
}
