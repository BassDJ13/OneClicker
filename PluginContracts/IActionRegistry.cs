namespace PluginContracts;

public interface IActionRegistry
{
    IList<PluginActionDescriptor> GetAllActions();
    PluginActionDescriptor? GetAction(string actionName);
}