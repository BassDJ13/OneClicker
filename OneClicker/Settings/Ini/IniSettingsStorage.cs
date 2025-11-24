using BassCommon.FileSystem;
using PluginContracts;

namespace OneClicker.Settings.Ini;

//todo: make each plugin responsible for it's own settings.
public class IniSettingsStorage : ISettingsStorage
{
    private readonly string _path;
    private readonly ISettings _settings;
    private readonly IFileSystem _fs;

    public bool FileExists { get; private set; }

    public IniSettingsStorage(string path, ISettings settings, IFileSystem? fs = null)
    {
        _path = path;
        _settings = settings;
        _fs = fs ?? new RealFileSystem();
    }

    public void Load()
    {
        if (!_fs.Exists(_path))
        {
            return;
        }
        FileExists = true;

        foreach (var line in _fs.ReadAllLines(_path))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            var (key, value) = (parts[0], parts[1]);

            switch (key)
            {
                case "Folder":
                    _settings.FolderPath = value;
                    break;

                case "X":
                    if (int.TryParse(value, out var x))
                    {
                        _settings.X = x;
                    }
                    break;

                case "Y":
                    if (int.TryParse(value, out var y))
                    {
                        _settings.Y = y;
                    }
                    break;

                case "WidgetSize":
                    if (int.TryParse(value, out var w))
                    {
                        _settings.WidgetSize = w;
                    }
                    break;

                case "BackColor": 
                    _settings.BackColor = ParseColor(value, _settings.BackColor);
                    break;

                case "ButtonColor":
                    _settings.ButtonColor = ParseColor(value, _settings.ButtonColor);
                    break;

                case "TriangleColor":
                    _settings.TriangleColor = ParseColor(value, _settings.TriangleColor);
                    break;

                case "WindowStyle":
                    if (Enum.TryParse<WindowStyle>(value, out var windowStyle))
                    {
                        _settings.WindowStyle = windowStyle;
                    }
                    break;

                case "DockPosition":
                    if (Enum.TryParse<DockPosition>(value, out var dockPosition))
                    {
                        _settings.DockPosition = dockPosition;
                    }
                    break;

                case "DockOffsetX":
                    if (int.TryParse(value, out var dockOffsetX)) _settings.DockOffsetX = dockOffsetX;
                    break;

                case "DockOffsetY":
                    if (int.TryParse(value, out var dockOffsetY)) _settings.DockOffsetY = dockOffsetY;
                    break;

                case "InactiveOpacity":
                    if (int.TryParse(value, out var inactiveOpacity)) _settings.InactiveOpacity = inactiveOpacity;
                    break;

                case "FocusShortcut":
                    _settings.FocusShortcut = value;
                    break;
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
            $"WidgetSize={_settings.WidgetSize}",
            $"BackColor={ColorToHex(_settings.BackColor)}",
            $"ButtonColor={ColorToHex(_settings.ButtonColor)}",
            $"TriangleColor={ColorToHex(_settings.TriangleColor)}",
            $"WindowStyle={_settings.WindowStyle}",
            $"DockPosition={_settings.DockPosition}",
            $"DockOffsetX={_settings.DockOffsetX}",
            $"DockOffsetY={_settings.DockOffsetY}",
            $"InactiveOpacity={_settings.InactiveOpacity}",
            $"FocusShortcut={_settings.FocusShortcut}"
        };
        _fs.WriteAllLines(_path, lines);
    }

    private static Color ParseColor(string hex, Color fallback)
    {
        try
        { 
            return ColorTranslator.FromHtml(hex);
        }
        catch 
        {
            return fallback;
        }
    }

    private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
