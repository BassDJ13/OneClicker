using BassCommon.Classes;
using Microsoft.Win32;
using OneClicker.Classes;
using OneClicker.Plugins;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using PluginContracts;

namespace OneClicker.Forms;

public class MainForm : Form, IMainWindow
{
    private readonly IAppSettings _settings;
    private readonly Panel _dragArea;
    private const int _dragAreaHeight = 6;
    private readonly Panel _contentPanel;
    private ISettingsStorage? _settingsIO;
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
            .OfType<IPluginWidgetBase>()
            .Select(widget => widget.StartAnimation())
            .ToList();
        await Task.WhenAll(tasks);
    }

    public MainForm()
    {
        AppServices.MainWindow = this;
        PluginManager.Initialize(this);
        Text = "OneClicker";
        _windowLocationHelper = new WindowLocationHelper(new ScreenProvider());
        _settings = AppSettings.Instance;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        BackColor = Color.MidnightBlue;

        InitializeSettings();

        TransparencyHelper.AttachAutoOpacity(this, ((double)_settings.InactiveOpacity) / 100f);

        _dragArea = new Panel
        {
            Dock = DockStyle.Top,
            Height = _dragAreaHeight,
            Cursor = Cursors.SizeAll,
            BackColor = Color.Transparent
        };
        _dragArea.MouseDown += (s, e) =>
        {
            if (_settings.WindowStyle == WindowStyle.Docked)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        };

        _dragArea.MouseUp += (s, e) =>
        {
            if (_settings.WindowStyle == WindowStyle.Docked)
                return;

            if (_isDragging)
            {
                _isDragging = false;
                _windowLocationHelper.KeepInWorkArea(this);
                _settings.X = Left;
                _settings.Y = Top;
                _settingsIO!.Save();
            }
        };

        _dragArea.MouseMove += (s, e) =>
        {
            if (_settings.WindowStyle == WindowStyle.Docked)
                return;

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

        BackColor = _settings.BackColor;
        DetermineAppSize();
        ApplyWindowStyle();

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void InitializeSettings()
    {
        _settingsIO = new IniSettingsStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"), _settings);
        _settingsIO.Load();
        if (!_settingsIO.FileExists)
        {
            SetDockedLocation();
            _settings.X = this.Left;
            _settings.Y = this.Top;
        }
    }

    private void DetermineAppSize()
    {
        _widgetSize = _settings.WidgetSize;
        var headerHeight = _settings.WindowStyle == WindowStyle.Floating ? _dragAreaHeight : 0;

        _appWidth = _widgetSize * Math.Max(1, PluginManager.Instance.ActiveWidgets.Count);
        _appHeight = _widgetSize + headerHeight;

        Size = new Size(_appWidth, _appHeight);
    }

    private void ApplyWindowStyle()
    {
        if (_settings.WindowStyle == WindowStyle.Docked)
        {
            _dragArea.Visible = false;
            SetDockedLocation();
        }
        else
        {
            _dragArea.Visible = true;
            Location = new Point(_settings.X, _settings.Y);
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
            var control = (UserControl)plugin.WidgetControl!;
            control.Dock = DockStyle.Fill; //todo: stack widgets horizontal
            _contentPanel.Controls.Add(control);
            plugin.WidgetControl!.ApplySettings();
            ((IPluginWidgetBase)control).RightClickDetected += (s, e) => ShowContextMenu(e);
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
        _hotkeyHelper = new GlobalHotkeyHelper(Handle, OnGlobalHotkeyPressed, KeyParser.FromSettingString(_settings.FocusShortcut));
        //todo: currently needs to restart app, fix this
    }

    private void OnGlobalHotkeyPressed()
    {
        ShowAndActivate(); 
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            plugin.WidgetControl?.ExecuteAction(); //todo: only execute one action for the preferred plugin
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
        using var dlg = new SettingsForm();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            BackColor = _settings.BackColor;
            DetermineAppSize();
            foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
            {
                plugin.WidgetControl?.ApplySettings();
            }
            ApplyWindowStyle();
            TransparencyHelper.SetInactiveOpacity(this, ((double)_settings.InactiveOpacity) / 100f);
            _settingsIO!.Save();
            Invalidate();
        }
    }

    private void SetDockedLocation()
    {
        var wa = Screen.FromHandle(Handle).WorkingArea;
        Location = _windowLocationHelper.GetDockedPosition(wa, Size, _settings.DockPosition, _settings.DockOffsetX, _settings.DockOffsetY);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        if (_settings.WindowStyle == WindowStyle.Floating)
        {
            _settings.X = Left;
            _settings.Y = Top;
            _settings.WidgetSize = _widgetSize;
        }
        _settingsIO!.Save();
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
