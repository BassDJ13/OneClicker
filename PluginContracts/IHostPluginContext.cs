namespace PluginContracts;

public interface IHostPluginContext
{
    public IActionRegistry ActionRegistry { get; }
    public IPluginRegistry PluginRegistry { get; }
}