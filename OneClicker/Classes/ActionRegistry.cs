using PluginContracts;

namespace OneClicker.Classes;

internal sealed class ActionRegistry : IActionRegistry
{
    private readonly IList<PluginActionDescriptor> _actions;

    public ActionRegistry(IList<PluginActionDescriptor> actions)
    {
        _actions = actions;
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
