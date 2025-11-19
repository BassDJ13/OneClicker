using PluginContracts;
using System.Reflection;

namespace OneClicker.Plugins;

public static class PluginLoader
{
    public static List<IPlugin> LoadPlugins(string folder, IMainWindow mainWindow)
    {
        var plugins = new List<IPlugin>();

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        foreach (var dll in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);

                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type) &&
                        !type.IsInterface && !type.IsAbstract)
                    {
                        var ctor = type.GetConstructor([typeof(IMainWindow)]);

                        IPlugin plugin = ctor != null
                            ? (IPlugin)ctor.Invoke([mainWindow])
                            : (IPlugin)Activator.CreateInstance(type)!;

                        plugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading plugin {dll}\n{ex.Message}");
            }
        }

        return plugins;
    }
}
