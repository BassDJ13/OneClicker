using BassCommon.FileSystem;
using PluginContracts;

namespace OneClicker.Settings.Ini;

public class IniSettingsStore : ISettingsStore
{
    private readonly string _path;
    private readonly IFileSystem _fs;
    private readonly Dictionary<string, string> _store = new();

    public bool FileExists { get; private set; }

    public IniSettingsStore(string path, IFileSystem? fs = null)
    {
        _path = path;
        _fs = fs ?? new RealFileSystem();
    }

    public void Load()
    {
        if (!_fs.Exists(_path))
        {
            return;
        }
        FileExists = true;

        foreach (var line in _fs.ReadAllLines(_path))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            _store.Add(parts[0], parts[1]);
        }
    }

    public void Save()
    {
        IList<string> lines = new List<string>();

        foreach (var kvp in _store)
        {
            lines.Add($"{kvp.Key}={kvp.Value}");
        }
        _fs.WriteAllLines(_path, lines);
    }

    public string? Get(string key) 
        => _store.TryGetValue(key, out string? value) ? value : null;

    public void Set(string key, string value)
        => _store[key] = value;
}
