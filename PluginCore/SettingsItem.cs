using PluginContracts;

namespace PluginCore;

public sealed class SettingsItem : ISettingsItem
{
    private readonly string _name;
    public string Name => _name;

    private Type? _settingsClass;
    public Type? SettingsClass => _settingsClass;

    private UserControl? _content;

    public UserControl? Content
    {
        get
        {
            if (SettingsClass == null)
            {
                return null;
            }

            if (_content != null && !_content.IsDisposed)
            {
                return _content;
            }

            _content = (UserControl)Activator.CreateInstance(SettingsClass)!;
            return _content;
        }
    }

    public SettingsItem(string name, Type? settingsClass)
    {
        _name = name;
        _settingsClass = settingsClass;
    }
}