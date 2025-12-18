using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Classes;

internal static class ContextMenuService
{
    public static void CreateMenuItemsForPlugins(ContextMenuStrip contextMenu)
    {
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            if (plugin.HasMenuItems)
            {
                var menuItem = contextMenu.Items.Add(plugin.Name);
                (menuItem as ToolStripMenuItem)!.DropDownItems.AddRange(CreateMenuItems(plugin.ContextMenuItems));
            }
        }
    }

    private static ToolStripMenuItem[] CreateMenuItems(IList<MenuItem> menuItems)
    {
        var result = new List<ToolStripMenuItem>();

        foreach (var item in menuItems)
        {
            var menuItem = new ToolStripMenuItem
            {
                Text = item.Description,
                Image = item.Image,
            };

            if (item.OnClick != null)
            {
                menuItem.Click += item.OnClick;
            }

            result.Add(menuItem);
        }

        return result.ToArray();
    }
}
