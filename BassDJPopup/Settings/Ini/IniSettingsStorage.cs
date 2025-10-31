using BassDJPopup.FileSystem;
using BassDJPopup.Settings;
using System.Runtime;

public class IniSettingsStorage : ISettingsStorage
{
    private readonly string _path;
    private readonly IAppSettings _settings;
    private readonly IFileSystem _fs;

    public IniSettingsStorage(string path, IAppSettings settings, IFileSystem? fs = null)
    {
        _path = path;
        _settings = settings;
        _fs = fs ?? new RealFileSystem(); // default implementation
    }

    public void Load()
    {
        if (!_fs.Exists(_path)) return;

        foreach (var line in _fs.ReadAllLines(_path))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;
            var (key, value) = (parts[0], parts[1]);

            switch (key)
            {
                case "Folder": _settings.FolderPath = value; break;
                case "X":
                    if (int.TryParse(value, out var x)) _settings.X = x;
                    break;

                case "Y":
                    if (int.TryParse(value, out var y)) _settings.Y = y;
                    break;

                case "Width":
                    if (int.TryParse(value, out var w)) _settings.Width = w;
                    break;

                case "Height":
                    if (int.TryParse(value, out var h)) _settings.Height = h;
                    break;
                case "BackColor": _settings.BackColor = ParseColor(value, _settings.BackColor); break;
                case "ButtonColor": _settings.ButtonColor = ParseColor(value, _settings.ButtonColor); break;
                case "TriangleColor": _settings.TriangleColor = ParseColor(value, _settings.TriangleColor); break;
            }
        }
    }

    public void Save()
    {
        var lines = new[]
        {
            $"Folder={_settings.FolderPath}",
            $"X={_settings.X}",
            $"Y={_settings.Y}",
            $"Width={_settings.Width}",
            $"Height={_settings.Height}",
            $"BackColor={ColorToHex(_settings.BackColor)}",
            $"ButtonColor={ColorToHex(_settings.ButtonColor)}",
            $"TriangleColor={ColorToHex(_settings.TriangleColor)}"
        };
        _fs.WriteAllLines(_path, lines);
    }

    private static Color ParseColor(string hex, Color fallback)
    {
        try { return ColorTranslator.FromHtml(hex); }
        catch { return fallback; }
    }

    private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
