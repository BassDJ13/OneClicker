using OneClicker.Forms;
using Microsoft.Extensions.DependencyInjection;
using OneClicker.WindowBehavior;
using OneClicker.Plugins;
using System.Runtime.InteropServices;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using BassCommon.FileSystem;
using PluginContracts;

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

        var services = new ServiceCollection();
        services.AddSingleton<IScreenProvider, ScreenProvider>();
        services.AddSingleton<IWindowLocationHelper, WindowLocationHelper>();
        services.AddSingleton<IPathProvider, PathProvider>();
        services.AddSingleton<IFileSystem, RealFileSystem>();
        services.AddSingleton<ISettingsStore, IniSettingsStore>();
        services.AddSingleton<IMainAppSettings>(sp => new MainAppSettings(sp.GetRequiredService<ISettingsStore>()));
        services.AddSingleton<IGlobalSettings>(sp => new GlobalSettings(sp.GetRequiredService<ISettingsStore>()));
        services.AddSingleton<IPluginManager>(sp => new PluginManager(sp.GetRequiredService<ISettingsStore>(), sp.GetRequiredService<IGlobalSettings>()));

        services.AddSingleton<WidgetsWindow>();

        using var serviceProvider = services.BuildServiceProvider();

        Application.Run(serviceProvider.GetRequiredService<WidgetsWindow>());
    }
}
