using PluginContracts;

namespace OneClicker.Classes;

internal sealed class ActionRegistry : IActionRegistry
{
    private readonly IList<PluginActionDescriptor> _actions;

    public ActionRegistry(IList<IPlugin> plugins)
    {
        _actions = new List<PluginActionDescriptor>();
        foreach (var plugin in plugins)
        {
            foreach (var action in plugin.Actions)
            {
                _actions.Add(new PluginActionDescriptor(
                    pluginId: plugin.Guid.ToString(),
                    actionId: action.Key,
                    pluginName: plugin.Name));
            }
        }
    }

    public PluginActionDescriptor? GetAction(string actionName)
    {
        foreach (var action in _actions)
        {
            if (action.UniqueKey == actionName)
            {
                return action;
            }
        }
        return null;
    }

    public IList<PluginActionDescriptor> GetAllActions()
        => _actions;
}
