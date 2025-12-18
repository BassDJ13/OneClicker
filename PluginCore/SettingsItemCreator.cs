using PluginContracts;

namespace PluginCore;

internal class SettingsItemCreator
{
    public IPluginSettings PluginSettings { get; }
    public IPluginSettings GlobalSettings { get; }

    private string _pluginId;

    public SettingsItemCreator(string pluginId, IPluginSettings pluginSettings, IPluginSettings globalSettings)
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
        _pluginId = pluginId;
    }

    public ISettingsItem Create(string name, Type? settingsClass)
    {
        return new SettingsItem(name, settingsClass, _pluginId, PluginSettings, GlobalSettings);
    }
}