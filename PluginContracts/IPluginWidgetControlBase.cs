namespace PluginContracts;

public interface IPluginWidgetControlBase
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();

    event EventHandler<MouseEventArgs>? RightClickDetected;
}