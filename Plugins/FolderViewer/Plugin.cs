using PluginContracts;

namespace FolderViewer;

public class Plugin : PluginBase
{
    public Plugin()
    {
        Name = "Folder Viewer";
        SettingsClass = typeof(Settings);
        WidgetClass = typeof(Widget);

        MenuItems.Add(new MenuItem(
            description: "Refresh Folder",
            image: null,
            onClick: (s, a) => PopupMenuProvider.Menu.Items.Clear()));
    }
}
