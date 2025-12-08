namespace PluginContracts;

public abstract class PluginWidgetBase : UserControl, IPluginWidgetBase
{
    public abstract Task StartAnimation();

    public abstract void ExecuteAction();

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
