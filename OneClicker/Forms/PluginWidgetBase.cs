using OneClicker.Settings;

namespace OneClicker.Forms;

public abstract class PluginWidgetBase : UserControl, IPluginWidget
{
    protected IAppSettings Settings => AppSettings.Instance;
    protected IMainWindow MainWindow { get; private set; }

    public abstract string MainMenuName { get; }

    public abstract ToolStripItem[] SubMenuItems { get; }

    public PluginWidgetBase(IMainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    public abstract Task StartAnimation();

    public abstract void ExecuteAction();

    public abstract void ApplySettings();
}
