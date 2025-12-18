using PluginContracts;

namespace PluginCore;

public sealed class SettingsItem : ISettingsItem 
{
    public IPluginSettings PluginSettings { get; private set; }
    public IPluginSettings GlobalSettings { get; private set; }

    private readonly string _name;
    public string Name => _name;

    private Type? _settingsClass;
    public Type? SettingsClass => _settingsClass;

    public string PluginId { get; private set; }

    private IPluginSettingsControlBase? _content;

    public IPluginSettingsControlBase? Content => _content;

    public IPluginSettingsControlBase? CreateContent(IPluginSettings pluginSettingsOverlay, IPluginSettings globalSettingsOverlay)
    {
        if (SettingsClass == null)
        {
            return null;
        }

        if (_content != null && !((PluginSettingsControlBase)_content).IsDisposed)
        {
            return _content;
        }

        _content = (IPluginSettingsControlBase)Activator.CreateInstance(SettingsClass, pluginSettingsOverlay, globalSettingsOverlay)!;

        return _content;
    }

    public SettingsItem(string name, Type? settingsClass, string pluginId, IPluginSettings pluginSettings, IPluginSettings globalSettings)
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
        PluginId = pluginId;
        _name = name;
        _settingsClass = settingsClass;
    }
}