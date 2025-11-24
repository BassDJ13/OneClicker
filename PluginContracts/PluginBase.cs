namespace PluginContracts;

public abstract class PluginBase : IPlugin
{
    protected Type? SettingsClass { get; set; }
    private UserControl? _settingsControl;

    public UserControl? SettingsControl
    {
        get
        {
            if (SettingsClass == null)
            {
                return null;
            }

            if (_settingsControl != null && !_settingsControl.IsDisposed)
            {
                return _settingsControl;
            }

            _settingsControl = (UserControl)Activator.CreateInstance(SettingsClass)!;
            return _settingsControl;
        }
    }

    public bool HasSettings => SettingsControl != null;

    protected Type? WidgetClass { get; set; }
    private PluginWidgetBase? _widgetControl;

    public PluginWidgetBase? WidgetControl =>
        _widgetControl ??= WidgetClass == null
            ? null
            : (PluginWidgetBase)Activator.CreateInstance(WidgetClass)!;

    public bool HasWidget => WidgetControl != null;

    public bool HasMenuItems => MenuItems.Count > 0;

    private IList<MenuItem>? _menuItems;
    public IList<MenuItem> MenuItems => _menuItems ??= new List<MenuItem>();

    public virtual string Name { get; protected set; } = "Unnamed Plugin";
    public virtual string Description { get; protected set; } = "";
}