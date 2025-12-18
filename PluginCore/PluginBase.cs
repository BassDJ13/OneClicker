using PluginContracts;

namespace PluginCore;

public abstract class PluginBase : IPlugin
{
    protected Type? WidgetClass { get; set; }

    private IPluginWidgetControlBase? _widgetInstance;
    public IPluginWidgetControlBase? WidgetInstance => _widgetInstance;

    private IList<MenuItem>? _contextMenuItems;
    public IList<MenuItem> ContextMenuItems => _contextMenuItems ??= new List<MenuItem>();

    public virtual string Name { get; protected set; } = "New";

    public IList<ISettingsItem> SettingsItems { get; protected set; } = new List<ISettingsItem>();

    public bool HasWidget => WidgetInstance != null;

    public bool HasSettings => SettingsItems != null && SettingsItems.Count > 0;

    public bool HasMenuItems => ContextMenuItems.Count > 0;

    internal SettingsItemCreator? SettingsItemCreator { get; private set; }

    protected IPluginSettings? PluginSettings { get; private set; }

    protected Dictionary<string, string> DefaultSettingValues { get; private set; }

    public PluginBase()
    {
        DefaultSettingValues = new Dictionary<string, string>();
    }

    public void Initialize(IPluginSettings pluginSettings, IPluginSettings globalSettings)
    {
        SettingsItemCreator = new SettingsItemCreator(Name, pluginSettings, globalSettings);

        if (WidgetClass != null)
        {
            _widgetInstance = (PluginWidgetControlBase)Activator.CreateInstance(WidgetClass, pluginSettings, globalSettings)!;
        }
        PluginSettings = pluginSettings;
        InitializePlugin();
        ProcessDefaultSettings();
    }

    public virtual void InitializePlugin()
    {
    }

    private void ProcessDefaultSettings()
    {
        foreach (var kvp in DefaultSettingValues)
        {
            if (PluginSettings!.Get(kvp.Key) == null)
            {
                PluginSettings.Set(kvp.Key, kvp.Value);
            }
        }
    }

    protected void AddSettingsItem(string name, Type? settingsClass)
    {
        SettingsItems.Add(SettingsItemCreator!.Create(name, settingsClass));
    }
}