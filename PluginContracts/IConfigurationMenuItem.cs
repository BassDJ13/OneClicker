namespace PluginContracts;

public interface IConfigurationMenuItem
{
    string PluginId { get; }
    string Name { get; }
    Type? ConfigurationClass { get; }
    IPluginConfigurationControl? Content {  get; }
    IPluginConfigurationControl? CreateConfigurationControl(IPluginContext context);
}