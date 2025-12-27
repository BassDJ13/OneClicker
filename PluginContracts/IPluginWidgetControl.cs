namespace PluginContracts;

public interface IPluginWidgetControl
{
    int WidthInUnits { get; }
    void SettingsChanged();
    Task StartAnimation();

    event EventHandler<MouseEventArgs>? OnRightMouseButtonUp;
}