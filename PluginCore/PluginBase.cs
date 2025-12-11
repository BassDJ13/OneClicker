using PluginContracts;

namespace PluginCore;

public abstract class PluginBase : IPlugin
{
    protected Type? WidgetClass { get; set; }
    private IPluginWidgetBase? _widgetInstance;

    public IPluginWidgetBase? WidgetInstance =>
        _widgetInstance ??= WidgetClass == null
            ? null
            : (PluginWidgetBase)Activator.CreateInstance(WidgetClass)!;

    public bool HasWidget => WidgetInstance != null;
    public bool HasSettings => SettingsItems != null && SettingsItems.Count > 0;

    public bool HasMenuItems => MenuItems.Count > 0;

    private IList<MenuItem>? _menuItems;
    public IList<MenuItem> MenuItems => _menuItems ??= new List<MenuItem>();

    public virtual string Name { get; protected set; } = "Unnamed Plugin";
    public virtual string Description { get; protected set; } = "";

    public IList<ISettingsItem> SettingsItems { get; protected set; } = new List<ISettingsItem>();
}