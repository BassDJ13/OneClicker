namespace OneClicker.Plugins;

public interface IPluginWidget
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();
}