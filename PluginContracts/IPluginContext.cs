namespace PluginContracts;

public interface IPluginContext
{
    public IPluginSettings PluginSettings { get; }
    public IGlobalSettings GlobalSettings { get; }
}