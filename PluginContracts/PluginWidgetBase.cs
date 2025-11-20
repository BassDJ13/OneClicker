namespace PluginContracts;

public abstract class PluginWidgetBase : UserControl, IPluginWidgetBase
{
    protected IAppSettings Settings => AppSettings.Instance;

    public abstract Task StartAnimation();

    public abstract void ExecuteAction();

    public abstract void ApplySettings();
}
