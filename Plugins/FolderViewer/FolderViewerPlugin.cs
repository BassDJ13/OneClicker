using PluginContracts;
using PluginCore;

namespace FolderViewer;

public class FolderViewerPlugin : PluginBase
{
    public FolderViewerPlugin()
    {
        Name = "Folder Viewer";
        WidgetClass = typeof(FolderViewerWidget);

        var widget = (FolderViewerWidget)WidgetInstance!;
        MenuItems.Add(new MenuItem(
            description: "Refresh Folder",
            image: null,
            onClick: widget.ClearMenu));

        SettingsItems.Add(new SettingsItem("Folder Viewer", typeof(FolderViewerSettings)));
    }
}
