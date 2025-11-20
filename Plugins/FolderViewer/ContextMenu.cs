using PluginContracts;

namespace FolderViewer;

public class ContextMenu : IPluginContextMenu
{
    public ToolStripItem[] SubMenuItems =>
    [
        new ToolStripMenuItem("Refresh Folder", null, (s, a) => PopupMenuService.PopupMenu.Items.Clear())
    ];
}
