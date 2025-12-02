using PluginContracts;
using PluginCore;

namespace FolderViewer;

public class FolderViewerPlugin : PluginBase
{
    public FolderViewerPlugin()
    {
        Name = "Folder Viewer";
        WidgetClass = typeof(Widget);

        MenuItems.Add(new MenuItem(
            description: "Refresh Folder",
            image: null,
            onClick: (s, a) => PopupMenuProvider.Menu.Items.Clear()));

        SettingsItems.Add(new SettingsItem("Folder Viewer", typeof(FolderViewerSettings)));
    }
}
