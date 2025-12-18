using PluginCore;

namespace FolderViewer;

public class FolderViewerPlugin : Plugin
{
    public override string Name => "Folder Viewer";

    protected override Type? WidgetClass => typeof(FolderViewerWidget);

    protected override void InitializeContextMenuItems()
    {
        var widget = (FolderViewerWidget)WidgetInstance!;
        AddContextMenuItem("Refresh Folder", null, widget.ClearMenu);
    }

    protected override void InitializeConfigurationControl()
    {
        AddConfigurationControl("Folder Viewer", typeof(FolderViewerConfiguration));
    }

    protected override void InitializePluginSettings()
    {
        DefaultSettingValues.Add("FolderPath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }
}
