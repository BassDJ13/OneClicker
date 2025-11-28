using OneClicker.Forms;
using System.Runtime.InteropServices;

namespace OneClicker;

internal static class Program
{
    private const string MutexName = "OneClicker_Mutex";
    private const uint WM_APP_SHOW = 0x8000 + 1;

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [STAThread]
    static void Main()
    {
        using var mutex = new Mutex(true, MutexName, out bool isNewInstance);

        if (!isNewInstance)
        {
            var existing = FindWindow(null, "OneClicker");
            if (existing != IntPtr.Zero)
            {
                PostMessage(existing, WM_APP_SHOW, IntPtr.Zero, IntPtr.Zero);
                SetForegroundWindow(existing);
            }
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.Run(new MainForm());
    }
}
