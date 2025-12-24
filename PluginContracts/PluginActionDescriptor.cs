namespace PluginContracts;

public sealed record PluginActionDescriptor
{
    public string PluginId { get; init; }
    public string ActionId { get; init; }
    public string DisplayName { get; init; }
    public string UniqueKey { get; init; }

    public PluginActionDescriptor(string pluginId, string actionId, string pluginName)
    {
        PluginId = pluginId;
        ActionId = actionId;
        DisplayName = string.IsNullOrWhiteSpace(actionId) 
            ? pluginName 
            : $"{pluginName}.{actionId}";
        UniqueKey = $"{pluginId}.{actionId}";
    }
}
