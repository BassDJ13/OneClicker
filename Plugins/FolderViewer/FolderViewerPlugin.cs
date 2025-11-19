using PluginContracts;

namespace FolderViewer;

public class FolderViewerPlugin : IPlugin
{
    private IMainWindow _mainWindow;
    private UserControl _settingsControl;
    private PluginWidgetBase _widgetControl;

    public string Name => "Folder Viewer";

    public FolderViewerPlugin(IMainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        //todo: with the current setup every control of every plugin will stay in memory as long as the program runs.
        _settingsControl = new FolderWidgetSettings();
        _widgetControl = new FolderWidget(_mainWindow); //todo: Are there other ways to interact with mainwindow? The only usecase so far is rightclick to open the contextmenu.
    }

    public UserControl SettingsControl => _settingsControl;

    public PluginWidgetBase WidgetControl => _widgetControl;
}
