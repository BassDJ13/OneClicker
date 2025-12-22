using PluginContracts;

namespace PluginCore;

public abstract class PluginWidgetControl : UserControl, IPluginWidgetControl
{
    public IPluginSettings PluginSettings { get; private set; }
    public IPluginSettings GlobalSettings { get; private set; }

    public PluginWidgetControl(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base()
    {
        PluginSettings = pluginSettings;
        GlobalSettings = globalSettings;
    }

    public abstract Task StartAnimation();

    public abstract void ApplySettings();

    public event EventHandler<MouseEventArgs>? RightClickDetected;

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButtons.Right)
        {
            RightClickDetected?.Invoke(this, e);
        }
    }
}
