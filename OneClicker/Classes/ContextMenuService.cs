
using System.Windows.Controls;
using PluginContracts;

namespace OneClicker.Classes;

internal static class ContextMenuService
{
    public static void CreateMenuItemsForPlugins(ContextMenu contextMenu, IList<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            if (plugin.HasContextMenuItems)
            {
                var menuItem = new MenuItem { Header = plugin.Name };
                foreach (var subItem in CreateMenuItems(plugin.ContextMenuItems))
                {
                    menuItem.Items.Add(subItem);
                }
                contextMenu.Items.Add(menuItem);
            }
        }
    }

    private static List<MenuItem> CreateMenuItems(IList<IContextMenuItem> menuItems)
    {
        var result = new List<MenuItem>();

        foreach (var item in menuItems)
        {
            var menuItem = new MenuItem
            {
                Header = item.Description
                // Icon = item.Image, // TODO: Convert to ImageSource if needed
            };

            if (item.OnClick != null)
            {
                menuItem.Click += (s, e) => item.OnClick(s, EventArgs.Empty);
            }

            result.Add(menuItem);
        }

        return result;
    }
}
