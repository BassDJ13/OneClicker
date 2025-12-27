namespace PluginContracts;

public interface IPluginWidgetControl
{
    void SettingsChanged();
    Task StartAnimation();

    event EventHandler<MouseEventArgs>? OnRightMouseButtonUp;
}