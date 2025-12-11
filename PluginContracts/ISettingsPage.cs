namespace PluginContracts;

//todo: becomes obsolete after settings refactor
public interface ISettingsPage
{
    void ReadFrom(ISettings settings);
    bool WriteTo(ISettings settings);
}