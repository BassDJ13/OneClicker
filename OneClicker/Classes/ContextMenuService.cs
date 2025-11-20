
using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Classes;

internal static class ContextMenuService
{
    public static void CreateMenuItemsForPlugins(ContextMenuStrip contextMenu)
    {
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            if (plugin.HasContextMenu)
            {
                var menuItem = contextMenu.Items.Add(plugin.Name);
                (menuItem as ToolStripMenuItem)!.DropDownItems.AddRange(plugin.ContextMenu!.SubMenuItems);
            }
        }
    }
}
