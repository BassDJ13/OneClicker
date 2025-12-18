using PluginContracts;
using PluginCore;

namespace FolderViewer;

public class FolderViewerPlugin : PluginBase
{
    public FolderViewerPlugin()
    {
        Name = "Folder Viewer";
        WidgetClass = typeof(FolderViewerWidget);
    }

    public override void InitializePlugin()
    {
        var widget = (FolderViewerWidget)WidgetInstance!;
        
        ContextMenuItems.Add(new MenuItem(
            description: "Refresh Folder",
            image: null,
            onClick: widget.ClearMenu));
        
        AddSettingsItem("Folder Viewer", typeof(FolderViewerSettings));

        DefaultSettingValues.Add("FolderPath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }
}
