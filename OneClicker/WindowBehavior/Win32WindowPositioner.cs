using System.Runtime.InteropServices;

namespace OneClicker.WindowBehavior;

public class Win32WindowPositioner : IWindowPositioner
{
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    private static readonly nint HWND_TOPMOST = new nint(-1);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOACTIVATE = 0x0010;

    public void SetTopMost(nint handle)
    {
        SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }
}
