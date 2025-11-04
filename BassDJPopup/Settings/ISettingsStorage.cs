namespace OneClicker.Settings;

public interface ISettingsStorage
{
    void Load();
    void Save();
    bool FileExists { get; }
}
