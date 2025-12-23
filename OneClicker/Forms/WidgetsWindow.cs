using BassCommon.Classes;
using Microsoft.Win32;
using OneClicker.Classes;
using OneClicker.Plugins;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using PluginContracts;

namespace OneClicker.Forms;

public class WidgetsWindow : Form
{
    private AppSettings? _mainAppSettings;
    private PluginSettingsProxy? _globalSettings;
    private readonly Panel _dragArea;
    private const int _dragAreaHeight = 6;
    private readonly Panel _contentPanel;
    private ISettingsStore? _settingsStore;
    private WindowLocationHelper _windowLocationHelper;
    private GlobalHotkeyHelper? _hotkeyHelper;
    private int _widgetSize = 16;
    private int _appWidth = 0;
    private int _appHeight = 0;
    private ContextMenuStrip? _contextMenu;
    private Action _shortcutAction; //todo: keep guid+actionNumber in _mainAppSettings

    public void Blink() => _ = BlinkAsync();

    private async Task BlinkAsync()
    {
        var tasks = _contentPanel.Controls
            .OfType<IPluginWidgetControl>()
            .Select(widget => widget.StartAnimation())
            .ToList();
        await Task.WhenAll(tasks);
    }

    public WidgetsWindow()
    {
        PluginManager.Initialize();
        Text = "OneClicker";
        _windowLocationHelper = new WindowLocationHelper(new ScreenProvider());
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        BackColor = Color.MidnightBlue;

        _settingsStore = new IniSettingsStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
        _settingsStore.Load();

        _mainAppSettings = new AppSettings(_settingsStore);
        _globalSettings = GlobalSettings.Initialize(_settingsStore); //todo: to much responsibility. The row below should not depend on calling Initialize here
        _shortcutAction = RetreivePluginAction(PluginManager.Instance.ActiveActions.First()); //todo: read setting instead of .First() when implemented

        if (!_settingsStore.FileExists)
        {
            SetDockedLocation();
            _mainAppSettings.X = this.Left;
            _mainAppSettings.Y = this.Top;
        }

        TransparencyHelper.AttachAutoOpacity(this, _mainAppSettings!.InactiveOpacity / 100f);

        _dragArea = new Panel
        {
            Dock = DockStyle.Top,
            Height = _dragAreaHeight,
            Cursor = Cursors.SizeAll,
            BackColor = Color.Transparent
        };

        _dragArea.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        };

        LocationChanged += (s, e) =>
        {
            _windowLocationHelper.KeepInWorkArea(this);
        };

        _contentPanel = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0)
        };

        Controls.Add(_contentPanel);
        Controls.Add(_dragArea);

        BackColor = _globalSettings!.GetColor(GlobalSettingKeys.HeaderColor, Color.MidnightBlue);
        DetermineAppSize();
        ApplyWindowStyle();

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private Action RetreivePluginAction(PluginActionDescriptor pluginActionDescriptor)
    {
        var plugin = PluginManager.Instance.GetPluginById(pluginActionDescriptor.PluginId);
        if (plugin.Actions.TryGetValue(pluginActionDescriptor.ActionId, out var action))
        {
            return action;
        }
        throw new NotImplementedException();
    }

    private void DetermineAppSize()
    {
        _widgetSize = _globalSettings!.GetInt(GlobalSettingKeys.WidgetSize, 16);
        var headerHeight = _mainAppSettings!.WindowStyle == WindowStyle.Floating ? _dragAreaHeight : 0;

        _appWidth = _widgetSize * Math.Max(1, PluginManager.Instance.ActiveWidgets.Count);
        _appHeight = _widgetSize + headerHeight;
        Size = new Size(_appWidth, _appHeight);
    }

    private void ApplyWindowStyle()
    {
        if (_mainAppSettings!.WindowStyle == WindowStyle.Docked)
        {
            _dragArea.Visible = false;
            SetDockedLocation();
        }
        else
        {
            _dragArea.Visible = true;
            Location = new Point(_mainAppSettings.X, _mainAppSettings.Y);
        }
        _windowLocationHelper.EnsureVisible(this);
        Blink();
    }

    private async Task ApplyWindowStyleAsync()
    {
        var screen = Screen.FromHandle(Handle);

        Rectangle wa;
        int attempts = 0;

        do
        {
            await Task.Delay(120);
            wa = screen.WorkingArea;
            attempts++;
        }
        while (IsFullScreenArea(wa, screen.Bounds) && attempts < 10);

        ApplyWindowStyle();
    }

    private bool IsFullScreenArea(Rectangle wa, Rectangle bounds)
    {
        return wa.Width == bounds.Width && wa.Height == bounds.Height;
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadWidgets();
        await BlinkAsync();
    }

    private void LoadWidgets()
    {
        _contentPanel.Controls.Clear();
        foreach (IPlugin plugin in PluginManager.Instance.ActiveWidgets)
        {
            var control = (UserControl)plugin.WidgetInstance!;
            control.Dock = DockStyle.Fill; //todo: stack widgets horizontal
            _contentPanel.Controls.Add(control);
            plugin.WidgetInstance!.ApplySettings();
            ((IPluginWidgetControl)control).RightClickDetected += (_, e) => ShowContextMenu(e);
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _hotkeyHelper = new GlobalHotkeyHelper(Handle, OnGlobalHotkeyPressed, KeyParser.FromSettingString(_mainAppSettings!.FocusShortcut));
        //todo: currently needs to restart app, fix this
    }

    private void OnGlobalHotkeyPressed()
    {
        ShowAndActivate(); 
        _shortcutAction?.Invoke();
    }

    private void ShowAndActivate()
    {
        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
        }
        Activate();
        Blink();
    }

    private const int WM_APP_SHOW = 0x8000 + 1;

    protected override void WndProc(ref Message m)
    {
        if (_hotkeyHelper != null && _hotkeyHelper.ProcessHotkeyMessage(ref m))
        {
            return;
        }

        if (m.Msg == WM_APP_SHOW)
        {
            ShowAndActivate();
            return;
        }

        base.WndProc(ref m);
        new TopMostHelper(new Win32WindowPositioner()).HandleMessage(this, ref m);
    }

    private bool ShowContextMenu(MouseEventArgs e)
    {
        if (_contextMenu != null)
        {
            _contextMenu.Show(this, e.Location);
            return false;
        }
        _contextMenu = new ContextMenuStrip();
        ContextMenuService.CreateMenuItemsForPlugins(_contextMenu);
        _contextMenu.Items.Add("Configuration", null, (s, a) => OpenConfiguration());
        _contextMenu.Items.Add("Close program", null, (s, a) => Close());

        _contextMenu.Show(this, e.Location);
        return true;
    }

    internal void OpenConfiguration()
    {
        using var dlg = new ConfigurationWindow(_settingsStore!);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            BackColor = _globalSettings!.GetColor(GlobalSettingKeys.HeaderColor, Color.MidnightBlue);
            DetermineAppSize();
            foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
            {
                plugin.WidgetInstance?.ApplySettings();
            }
            ApplyWindowStyle();
            TransparencyHelper.SetInactiveOpacity(this, ((double)_mainAppSettings!.InactiveOpacity) / 100f);
            _settingsStore!.Save();
            Invalidate();
        }
    }

    private void SetDockedLocation()
    {
        var wa = Screen.FromHandle(Handle).WorkingArea;
        Location = _windowLocationHelper.GetDockedPosition(wa, Size, _mainAppSettings!.DockPosition, _mainAppSettings!.DockOffsetX, _mainAppSettings!.DockOffsetY);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _mainAppSettings!.X = Left;
        _mainAppSettings!.Y = Top;
        _settingsStore!.Save();
        base.OnFormClosing(e);
    }

    private async void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        await ApplyWindowStyleAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _hotkeyHelper?.Dispose();
        base.OnFormClosed(e);
    }

    private static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern int SendMessage(nint hWnd, int msg, int wParam, int lParam);
    }
}
