using PluginContracts;

namespace OneClicker.Plugins;

internal sealed class PluginContext : IPluginContext, IHostPluginContext
{
    public IPluginSettings PluginSettings { get; }
    public IGlobalSettings GlobalSettings { get; }
    public IActionRegistry ActionRegistry { get; }
    public IPluginRegistry PluginRegistry { get; }

    public PluginContext(IPluginSettings pluginSettings, IGlobalSettings globalSettings, IActionRegistry actionRegistry, IPluginRegistry pluginRegistry)
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
        ActionRegistry = actionRegistry;
        PluginRegistry = pluginRegistry;
    }
}