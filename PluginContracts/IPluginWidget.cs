namespace PluginContracts;

public interface IPluginWidget
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();
}