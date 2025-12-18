using BassCommon.FileSystem;
using PluginContracts;
using System.Text.Json;

namespace OneClicker.Settings.Json;

public class JsonSettingsStorage : ISettingsStore
{
    //private readonly string _path;
    //private readonly ISettings _settings;
    //private readonly IFileSystem _fs;
    private readonly JsonSerializerOptions _opts = new()
    {
        WriteIndented = true,
        Converters = { new JsonColorConverter() }
    };

    public bool FileExists { get; private set; }

    //public JsonSettingsStorage(string path, ISettings settings, IFileSystem? fs = null)
    //{
    //    _path = path;
    //    _settings = settings;
    //    _fs = fs ?? new RealFileSystem();
    //}

    public void Load()
    {
        //if (!_fs.Exists(_path))
        //{
        //    return;
        //}
        //FileExists = true;

        //var json = string.Join('\n', _fs.ReadAllLines(_path));
        //var loaded = JsonSerializer.Deserialize<AppSettings>(json, _opts);
        //if (loaded != null)
        //{
        //    _settings.FolderPath = loaded.FolderPath;
        //    _settings.X = loaded.X;
        //    _settings.Y = loaded.Y;
        //    _settings.WidgetSize = loaded.WidgetSize;
        //    _settings.BackColor = loaded.BackColor;
        //    _settings.ButtonColor = loaded.ButtonColor;
        //    _settings.TriangleColor = loaded.TriangleColor;
        //}
    }

    public void Save()
    {
        //var json = JsonSerializer.Serialize((AppSettings)_settings, _opts);
        //_fs.WriteAllLines(_path, json.Split('\n'));
    }

    public string? Get(string key)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }
}
