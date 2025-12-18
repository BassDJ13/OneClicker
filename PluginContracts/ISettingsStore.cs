namespace PluginContracts;

public interface ISettingsStore
{
    string? Get(string key);
    void Set(string key, string value);
    void Load();
    void Save();
    bool FileExists { get; }
}
