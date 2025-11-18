namespace OneClicker.Forms;

public interface IPluginWidget
{
    void ApplySettings();
    Task StartAnimation();
    void ExecuteAction();

    string MainMenuName { get; }

    ToolStripItem[] SubMenuItems { get; }
}