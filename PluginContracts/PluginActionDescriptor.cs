namespace PluginContracts;

public sealed record PluginActionDescriptor
{
    public string PluginId { get; init; }
    public string ActionId { get; init; }
    public string DisplayName { get; init; }

    public PluginActionDescriptor(string pluginId, string actionId, string displayName)
    {
        PluginId = pluginId;
        ActionId = actionId;
        DisplayName = displayName;
    }
}
