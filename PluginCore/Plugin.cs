using PluginContracts;

namespace PluginCore;

public abstract class Plugin : IPlugin
{
    protected virtual Type? WidgetClass { get; }

    private IPluginWidgetControl? _widgetInstance;
    public IPluginWidgetControl? WidgetInstance => _widgetInstance;

    private IList<IContextMenuItem>? _contextMenuItems;
    public IList<IContextMenuItem> ContextMenuItems => _contextMenuItems ??= [];

    public abstract Guid Guid { get; }

    public abstract string Name { get; }

    public IList<IConfigurationMenuItem> ConfigurationMenuItems { get; protected set; } = [];

    public bool HasWidget => WidgetInstance != null;

    public bool HasConfiguration => ConfigurationMenuItems != null && ConfigurationMenuItems.Count > 0;

    public bool HasContextMenuItems => ContextMenuItems.Count > 0;

    private ConfigurationMenuItemCreator? _configurationMenuItemCreator;

    protected IPluginSettings? PluginSettings { get; private set; }

    private readonly Dictionary<string, string> _defaultSettingValues;

    public Dictionary<string, Action> Actions { get; } = [];

    public Plugin()
    {
        _defaultSettingValues = []; 
    }

    public void PreInitialize(IPluginSettings pluginSettings, IGlobalSettings globalSettings)
    {
        _configurationMenuItemCreator = new ConfigurationMenuItemCreator(Name);

        if (WidgetClass != null)
        {
            _widgetInstance = (PluginWidgetControl)Activator.CreateInstance(WidgetClass, pluginSettings, globalSettings)!;
        }

        PluginSettings = pluginSettings;
        InitializeContextMenuItems();
        InitializeSettings();
        InitializeActions();
        ProcessDefaultSettings();
    }

    public void PostInitialize()
    {
        InitializeConfigurationControls();
    }

    protected virtual void InitializeContextMenuItems()
    {
    }

    protected virtual void InitializeConfigurationControls()
    {
    }

    protected virtual void InitializeSettings()
    {
    }

    protected virtual void InitializeActions()
    {
    }

    protected void AddSetting(string name, string defaultValue)
    {
        _defaultSettingValues.Add(name, defaultValue);
    }

    private void ProcessDefaultSettings()
    {
        foreach (var kvp in _defaultSettingValues)
        {
            if (PluginSettings!.Get(kvp.Key) == null)
            {
                PluginSettings.Set(kvp.Key, kvp.Value);
            }
        }
    }

    protected void AddConfigurationControl(string name, Type? configureClass, params object[] customParameters)
    {
        ConfigurationMenuItems.Add(_configurationMenuItemCreator!.Create(name, configureClass, customParameters));
    }

    protected void AddContextMenuItem(string description, Image? image, Action onClick)
    {
        ContextMenuItems.Add(new ContextMenuItem(description, image, onClick));
    }

    protected void AddAction(string name, Action action)
    {
        Actions.Add(name, action);
    }
}