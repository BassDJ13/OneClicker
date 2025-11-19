namespace PluginContracts;

public interface IPluginContextMenu
{
    string MainMenuName { get; }
    ToolStripItem[] SubMenuItems { get; }
}