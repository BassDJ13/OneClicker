using PluginContracts;

namespace OneClicker.WindowBehavior;

public class WindowLocationHelper
{
    private readonly IScreenProvider _screenProvider;

    public WindowLocationHelper(IScreenProvider screenProvider)
    {
        _screenProvider = screenProvider;
    }

    public void EnsureVisible(Form form) => KeepInWorkArea(form);

    public void KeepInWorkArea(Form form)
    {
        var wa = _screenProvider.GetWorkingArea(form);

        int x = Math.Max(wa.Left, Math.Min(form.Left, wa.Right - form.Width));
        int y = Math.Max(wa.Top, Math.Min(form.Top, wa.Bottom - form.Height));

        form.Location = new Point(x, y);
    }

    public Point GetDockedPosition(Rectangle workingArea, Size windowSize, DockPosition position, int offsetX, int offsetY)
    {
        int x = 0, y = 0;

        switch (position)
        {
            case DockPosition.TopLeft:
                x = workingArea.Left;
                y = workingArea.Top;
                break;

            case DockPosition.Top:
                x = workingArea.Left + (workingArea.Width - windowSize.Width) / 2;
                y = workingArea.Top;
                break;

            case DockPosition.TopRight:
                x = workingArea.Right - windowSize.Width;
                y = workingArea.Top;
                break;

            case DockPosition.Left:
                x = workingArea.Left;
                y = workingArea.Top + (workingArea.Height - windowSize.Height) / 2;
                break;

            case DockPosition.Right:
                x = workingArea.Right - windowSize.Width;
                y = workingArea.Top + (workingArea.Height - windowSize.Height) / 2;
                break;

            case DockPosition.BottomLeft:
                x = workingArea.Left;
                y = workingArea.Bottom - windowSize.Height;
                break;

            case DockPosition.Bottom:
                x = workingArea.Left + (workingArea.Width - windowSize.Width) / 2;
                y = workingArea.Bottom - windowSize.Height;
                break;

            case DockPosition.BottomRight:
                x = workingArea.Right - windowSize.Width;
                y = workingArea.Bottom - windowSize.Height;
                break;
        }

        return new Point(x + offsetX, y + offsetY);
    }
}
