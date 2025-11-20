using PluginContracts;

namespace FolderViewer;

public class Plugin : PluginBase
{
    public Plugin()
    {
        Name = "Folder Viewer";
        SettingsClass = typeof(Settings);
        WidgetClass = typeof(Widget);
        ContextMenuClass = typeof(ContextMenu);
    }
}
