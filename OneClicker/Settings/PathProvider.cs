namespace OneClicker.Settings;

public class PathProvider : IPathProvider
{
    public string GetConfigPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
    }
}
