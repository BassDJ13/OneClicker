namespace PluginContracts;

public interface IPluginWidgetControl
{
    void ApplySettings();
    Task StartAnimation();

    event EventHandler<MouseEventArgs>? OnRightMouseButtonUp;
}