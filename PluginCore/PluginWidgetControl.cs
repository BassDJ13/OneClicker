using PluginContracts;

namespace PluginCore;

public abstract class PluginWidgetControl : UserControl, IPluginWidgetControl
{
    public IPluginSettings PluginSettings { get; private set; }
    public IGlobalSettings GlobalSettings { get; private set; }

    public PluginWidgetControl(IPluginSettings pluginSettings, IGlobalSettings globalSettings) : base()
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
    }

    public abstract Task StartAnimation();

    public abstract void SettingsChanged();

    public event EventHandler<MouseEventArgs>? OnRightMouseButtonUp;

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButtons.Right)
        {
            OnRightMouseButtonUp?.Invoke(this, e);
        }
    }
}
