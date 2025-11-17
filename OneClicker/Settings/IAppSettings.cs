namespace OneClicker.Settings;

public interface IAppSettings : ISettings
{
    static abstract IAppSettings Instance { get; }
    ISettings Copy();
    void Save(ISettings settings);
}