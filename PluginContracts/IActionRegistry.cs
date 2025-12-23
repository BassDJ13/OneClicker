namespace PluginContracts;

public interface IActionRegistry
{
    IReadOnlyList<PluginActionDescriptor> GetAllActions();
}