namespace PluginContracts;

public abstract class PluginWidgetBase : UserControl, IPluginWidget
{
    protected IAppSettings Settings => AppSettings.Instance;
    protected IMainWindow MainWindow { get; private set; }

    public PluginWidgetBase(IMainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    public abstract Task StartAnimation();

    public abstract void ExecuteAction();

    public abstract void ApplySettings();
}
