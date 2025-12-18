using BassCommon.Classes;
using Microsoft.Win32;
using OneClicker.Classes;
using OneClicker.Plugins;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using PluginContracts;

namespace OneClicker.Forms;

public class MainForm : Form
{
    private AppSettings? _mainAppSettings;
    private PluginSettingsProxy? _globalSettings;
    private readonly Panel _dragArea;
    private const int _dragAreaHeight = 6;
    private readonly Panel _contentPanel;
    private ISettingsStore? _settingsStore;
    private bool _isDragging = false;
    private WindowLocationHelper _windowLocationHelper;
    private GlobalHotkeyHelper? _hotkeyHelper;
    private int _widgetSize = 16;
    private int _appWidth = 0;
    private int _appHeight = 0;
    private ContextMenuStrip? _contextMenu;

    public void Blink() => _ = BlinkAsync();

    private async Task BlinkAsync()
    {
        var tasks = _contentPanel.Controls
            .OfType<IPluginWidgetControlBase>()
            .Select(widget => widget.StartAnimation())
            .ToList();
        await Task.WhenAll(tasks);
    }

    public MainForm()
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

        InitializeSettings();

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
            if (_mainAppSettings.WindowStyle == WindowStyle.Docked)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        };

        _dragArea.MouseUp += (s, e) =>
        {
            if (_mainAppSettings.WindowStyle == WindowStyle.Docked)
            {
                return;
            }

            if (_isDragging)
            {
                _isDragging = false;
                _windowLocationHelper.KeepInWorkArea(this);
                _mainAppSettings.X = Left;
                _mainAppSettings.Y = Top;
                _settingsStore!.Save();
            }
        };

        _dragArea.MouseMove += (s, e) =>
        {
            if (_mainAppSettings.WindowStyle == WindowStyle.Docked)
            {
                return;
            }

            if (_isDragging)
            {
                _windowLocationHelper.KeepInWorkArea(this);
            }
        };

        _contentPanel = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0)
        };

        Controls.Add(_contentPanel);
        Controls.Add(_dragArea);

        BackColor = _globalSettings!.GetColor(GlobalSettingKeys.BackColor, Color.MidnightBlue);
        DetermineAppSize();
        ApplyWindowStyle();

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void InitializeSettings()
    {
        _settingsStore = new IniSettingsStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
        _settingsStore.Load();

        _mainAppSettings = new AppSettings(_settingsStore);
        _globalSettings = new PluginSettingsProxy("Global", _settingsStore);

        SetDefaultGlobalSettings(_globalSettings);

        foreach (var plugin in PluginManager.Instance.ActivePlugins)
        {
            var settingsProxy = new PluginSettingsProxy(plugin.Name, _settingsStore);
            plugin.Initialize(settingsProxy, _globalSettings);
        }

        if (!_settingsStore.FileExists)
        {
            SetDockedLocation();
            _mainAppSettings.X = this.Left;
            _mainAppSettings.Y = this.Top;
        }
    }

    private void SetDefaultGlobalSettings(PluginSettingsProxy globalSettings)
    {
        var _defaultSettingValues = new Dictionary<string, string>
        {
            { "WidgetSize", "16" },
            { GlobalSettingKeys.BackColor, ColorToHex(Color.MidnightBlue) },
            { GlobalSettingKeys.ButtonColor, ColorToHex(Color.SteelBlue) },
            { GlobalSettingKeys.TriangleColor, ColorToHex(Color.LightBlue) }
        };

        foreach (var kvp in _defaultSettingValues)
        {
            if (globalSettings!.Get(kvp.Key) == null)
            {
                globalSettings.Set(kvp.Key, kvp.Value);
            }
        }
    }

    private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

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
            ((IPluginWidgetControlBase)control).RightClickDetected += (_, e) => ShowContextMenu(e);
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
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            plugin.WidgetInstance?.ExecuteAction(); //todo: only execute one action for the preferred plugin
        }
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
        _contextMenu.Items.Add("App settings", null, (s, a) => ShowSettings());
        _contextMenu.Items.Add("Close program", null, (s, a) => Close());

        _contextMenu.Show(this, e.Location);
        return true;
    }

    internal void ShowSettings()
    {
        using var dlg = new SettingsForm(_settingsStore!);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            BackColor = _globalSettings!.GetColor(GlobalSettingKeys.BackColor, Color.MidnightBlue);
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
        if (_mainAppSettings!.WindowStyle == WindowStyle.Floating)
        {
            _mainAppSettings.X = Left;
            _mainAppSettings.Y = Top;
            _globalSettings!.SetInt(GlobalSettingKeys.WidgetSize, _widgetSize);
        }
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
