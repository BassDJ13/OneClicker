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

            return (UserControl)Activator.CreateInstance(SettingsClass)!;
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

    protected Type? ContextMenuClass { get; set; }
    private IPluginContextMenu? _contextMenu;

    public IPluginContextMenu? ContextMenu =>
        _contextMenu ??= ContextMenuClass == null
            ? null
            : (IPluginContextMenu)Activator.CreateInstance(ContextMenuClass)!;

    public bool HasContextMenu => ContextMenu != null;

    public virtual string Name { get; protected set; } = "Unnamed Plugin";
    public virtual string Description { get; protected set; } = "";
}

