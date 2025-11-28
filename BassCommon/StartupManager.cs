namespace BassCommon;

public static class StartupManager
{
    private static string GetShortcutPath(string shortcutName)
    {
        var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        return Path.Combine(startupFolder, $"{shortcutName}.lnk");
    }

    public static bool EnableStartup(string shortcutName)
    {
        var shortcutPath = GetShortcutPath(shortcutName);

        if (File.Exists(shortcutPath))
        {
            return false;
        }

        var wshShellType = Type.GetTypeFromProgID("WScript.Shell");
        if (wshShellType == null)
        {
            return false;
        }

        dynamic? wshShell = Activator.CreateInstance(wshShellType);
        if (wshShell == null)
        {
            return false;
        }

        try
        {
            dynamic shortcut = wshShell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            shortcut.Save();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void DisableStartup(string shortcutName)
    {
        var shortcutPath = GetShortcutPath(shortcutName);

        if (File.Exists(shortcutPath))
        {
            File.Delete(shortcutPath);
        }
    }

    public static bool IsStartupEnabled(string shortcutName)
    {
        var shortcutPath = GetShortcutPath(shortcutName);
        return File.Exists(shortcutPath);
    }
}
