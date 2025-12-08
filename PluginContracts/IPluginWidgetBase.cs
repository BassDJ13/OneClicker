namespace PluginContracts;

public interface IPluginWidgetBase
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();

    event EventHandler<MouseEventArgs>? RightClickDetected;

}