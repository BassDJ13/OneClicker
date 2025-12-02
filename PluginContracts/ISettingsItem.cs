namespace PluginContracts
{
    public interface ISettingsItem
    {
        string Name { get; }
        Type? SettingsClass { get; }
        UserControl? Content {  get; }
    }
}