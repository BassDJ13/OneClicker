namespace PluginContracts;

public interface IPluginWidgetControl
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();

    event EventHandler<MouseEventArgs>? RightClickDetected;
}