using PluginContracts;

namespace OneClicker.Classes;

internal sealed class ActionRegistry : IActionRegistry
{
    private readonly List<PluginActionDescriptor> _actions;

    public ActionRegistry(List<PluginActionDescriptor> actions)
    {
        _actions = actions;
    }

    public IReadOnlyList<PluginActionDescriptor> GetAllActions()
        => _actions;
}
