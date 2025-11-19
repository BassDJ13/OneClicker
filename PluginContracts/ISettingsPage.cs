namespace PluginContracts;

public interface ISettingsPage
{
    void ReadFrom(ISettings settings);
    bool WriteTo(ISettings settings);
}