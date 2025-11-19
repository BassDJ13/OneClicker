namespace OneClicker.Forms;

public interface IPluginContextMenu
{
    string MainMenuName { get; }
    ToolStripItem[] SubMenuItems { get; }
}