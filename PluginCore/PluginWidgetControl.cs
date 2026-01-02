using PluginContracts;

namespace PluginCore;

public abstract class PluginWidgetControl : UserControl, IPluginWidgetControl
{
    public int WidthInUnits { get; }
    protected IPluginSettings PluginSettings { get; private set; }
    protected IGlobalSettings GlobalSettings { get; private set; }

    public PluginWidgetControl(IPluginContext context) : base()
    {
        PluginSettings = context.PluginSettings;
        GlobalSettings = context.GlobalSettings;
        WidthInUnits = GetWidthInUnits();
    }

    public abstract Task StartAnimation();

    public abstract void SettingsChanged();

    protected virtual int GetWidthInUnits() => 1;

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