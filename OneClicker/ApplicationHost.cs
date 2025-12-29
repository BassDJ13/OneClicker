using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneClicker.Forms;
using OneClicker.Plugins;

namespace OneClicker;

public static class ApplicationHost
{
    public static IHost Build()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Core services
                services.AddSingleton<PluginManager>();

                // Forms
                services.AddTransient<WidgetsWindow>();
                services.AddTransient<ConfigurationWindow>();
            })
            .Build();
    }
}
