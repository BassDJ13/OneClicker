using PluginContracts;

namespace OneClicker.WindowBehavior;

public class WindowLocationHelper
{
    private readonly IScreenProvider _screenProvider;

    public WindowLocationHelper(IScreenProvider screenProvider)
    {
        _screenProvider = screenProvider;
    }
}
