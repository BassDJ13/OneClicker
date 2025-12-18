namespace PluginContracts;

public interface ISettingsItem
{
    string PluginId { get; }
    string Name { get; }
    Type? SettingsClass { get; }
    IPluginSettingsControlBase? Content {  get; }
    IPluginSettingsControlBase? CreateContent(IPluginSettings overlay, IPluginSettings globalSettings);
}