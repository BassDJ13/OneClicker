using OneClicker.Forms;
using OneClicker.Plugins;
using OneClicker.Settings;

namespace OneClicker.Classes;

internal class PluginManager
{

    public IList<IPluginWidget> ActivePlugins;

    public PluginManager(IMainWindow mainWindow)
    {
        ActivePlugins =
        [
            new FolderWidget(mainWindow)
        ];
    }
}
