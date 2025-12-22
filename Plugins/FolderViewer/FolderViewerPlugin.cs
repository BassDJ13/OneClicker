using PluginCore;

namespace FolderViewer;

public class FolderViewerPlugin : Plugin
{
    public override Guid Guid => new Guid("8009CBBE-12D8-442A-BF7B-4E2710F36DF7");
    public override string Name => "Folder Viewer";

    protected override Type? WidgetClass => typeof(FolderViewerWidget);

    FolderViewerWidget Widget => (FolderViewerWidget)WidgetInstance!;

    protected override void InitializeContextMenuItems()
    {
        AddContextMenuItem("Refresh Folder", null, Widget.ClearMenu);
    }

    protected override void InitializeConfigurationControls()
    {
        AddConfigurationControl("Folder Viewer", typeof(FolderViewerConfiguration));
    }

    protected override void InitializeSettings()
    {
        var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        AddSetting(SettingKeys.FolderPath, defaultPath);
    }

    protected override void InitializeActions()
    {
        AddAction("Display folder", Widget.DisplayFolder);
    }
}
