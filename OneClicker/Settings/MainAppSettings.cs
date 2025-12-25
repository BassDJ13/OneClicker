using OneClicker.Plugins;
using PluginContracts;

namespace OneClicker.Settings;

internal class MainAppSettings : PluginSettingsProxy
{
    private const string _prefix = "App";
    private const string _inactiveOpacity = "InactiveOpacity";
    private const string _windowStyle = "WindowStyle";
    private const string _dockPosition = "DockPosition";
    private const string _dockOffsetX = "DockOffsetX";
    private const string _dockOffsetY = "DockOffsetY";
    private const string _x = "X";
    private const string _y = "Y";
    private const string _focusShortcut = "FocusShortcut";
    private const string _shortcutAction = "ShortcutAction";

    internal double InactiveOpacity
    {
        get => this.GetDouble(_inactiveOpacity, 50);
        set => this.SetDouble(_inactiveOpacity, value);
    }

    internal WindowStyle WindowStyle
    {
        get => Enum.TryParse(Get(_windowStyle), out WindowStyle result)
            ? result
            : WindowStyle.Floating;
        set => Set(_windowStyle, nameof(value));
    }

    internal DockPosition DockPosition
    {
        get => Enum.TryParse(Get(_dockPosition), out DockPosition result)
            ? result
            : DockPosition.BottomRight;
        set => Set(_dockPosition, nameof(value));
    }

    internal int DockOffsetX
    {
        get => this.GetInt(_dockOffsetX, -24);
        set => this.SetInt(_dockOffsetX, value);
    }

    internal int DockOffsetY
    {
        get => this.GetInt(_dockOffsetY, -4);
        set => this.SetInt(_dockOffsetY, value);
    }

    internal int X
    {
        get => this.GetInt(_x, 24);
        set => this.SetInt(_x, value);
    }

    internal int Y
    {
        get => this.GetInt(_y, 4);
        set => this.SetInt(_y, value);
    }

    internal string FocusShortcut
    {
        get => Get(_focusShortcut);
        set => Set(_focusShortcut, value);
    }

    internal string ShortcutAction
    {
        get => Get(_shortcutAction);
        set => Set(_shortcutAction, value);
    }

    internal MainAppSettings(ISettingsStore store) : base(_prefix, store)
    {
    }
}
