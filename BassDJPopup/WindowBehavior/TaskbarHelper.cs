namespace BassDJPopup.WindowBehavior;

public class TaskbarHelper
{
    private readonly IScreenProvider _screenProvider;

    public TaskbarHelper(IScreenProvider screenProvider)
    {
        _screenProvider = screenProvider;
    }

    public void KeepInWorkArea(Form form)
    {
        var wa = _screenProvider.GetWorkingArea(form);

        int x = Math.Max(wa.Left, Math.Min(form.Left, wa.Right - form.Width));
        int y = Math.Max(wa.Top, Math.Min(form.Top, wa.Bottom - form.Height));

        form.Location = new Point(x, y);
    }

    public void DockAboveTaskbar(Form form)
    {
        var wa = _screenProvider.GetWorkingArea(form);
        form.Left = wa.Right - form.Width - 16;
        form.Top = wa.Bottom - form.Height - 4;
    }

    public void EnsureVisible(Form form) => KeepInWorkArea(form);
}
