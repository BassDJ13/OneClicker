namespace PluginContracts;

public interface IPluginSettings
{
    string? Get(string key);
    void Set(string key, string value);
}