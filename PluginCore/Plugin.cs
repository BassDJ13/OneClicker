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

    private readonly Dictionary<string, string> _defaultSettingValues;

    public Dictionary<string, Action> Actions { get; } = [];

    public Plugin()
    {
        _defaultSettingValues = [];
    }

    public void PreInitialize(IPluginContext context)
    {
        _configurationMenuItemCreator = new ConfigurationMenuItemCreator(Name);

        if (WidgetClass != null)
        {
            _widgetInstance = (PluginWidgetControl)Activator.CreateInstance(WidgetClass, [context])!;
        }

        InitializeContextMenuItems();
        InitializeSettings();
        InitializeActions();
        ProcessDefaultSettings(context.PluginSettings);
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

    private void ProcessDefaultSettings(IPluginSettings pluginSettings)
    {
        foreach (var kvp in _defaultSettingValues)
        {
            if (pluginSettings!.Get(kvp.Key) == null)
            {
                pluginSettings.Set(kvp.Key, kvp.Value);
            }
        }
    }

    protected void AddConfigurationControl(string name, Type? configureClass)
    {
        ConfigurationMenuItems.Add(_configurationMenuItemCreator!.Create(name, configureClass));
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