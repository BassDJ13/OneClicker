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
    private MainAppSettings? _mainAppSettings;
    private GlobalSettings? _globalSettings;
    private readonly Panel _dragArea;
    private const int _dragAreaHeight = 6;
    private readonly Panel _contentPanel;
    private ISettingsStore? _settingsStore;
    private WindowLocationHelper _windowLocationHelper;
    private GlobalHotkeyHelper? _hotkeyHelper;
    private ContextMenuStrip? _contextMenu;

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
        Text = "OneClicker";
        _windowLocationHelper = new WindowLocationHelper(new ScreenProvider());
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;

        _settingsStore = new IniSettingsStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
        _settingsStore.Load();

        _mainAppSettings = new MainAppSettings(_settingsStore);
        _globalSettings = new GlobalSettings(_settingsStore);
        PluginManager.Instance.InitializePlugins(_settingsStore, _globalSettings);

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
            BackColor = Color.Transparent,
            Padding = new Padding(0),
            Margin = new Padding(0)
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
            AutoSize = false,
            Dock = DockStyle.Fill,
            Padding = new Padding(0),
            Margin = new Padding(0),
        };

        Controls.Add(_contentPanel);
        Controls.Add(_dragArea);

        BackColor = _globalSettings!.HeaderColor;
        RefreshLayout();

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void RefreshLayout()
    {
        var widgetSize = _globalSettings!.WidgetSize;
        var headerHeight = _mainAppSettings!.WindowStyle == WindowStyle.Floating ? _dragAreaHeight : 0;

        var appWidth = widgetSize * Math.Max(1, PluginManager.Instance.WidthOfWidgetsInUnits());
        var appHeight = widgetSize + headerHeight;
        Size = new Size(appWidth, appHeight);

        ApplyWindowStyle();
        RefreshLayoutOfWidgets();
        Blink();
    }

    private void ApplyWindowStyle()
    {
        if (_mainAppSettings!.WindowStyle == WindowStyle.Docked)
        {
            _dragArea.Visible = false;
            _dragArea.Height = 0;
            SetDockedLocation();
        }
        else
        {
            _dragArea.Visible = true;
            _dragArea.Height = _dragAreaHeight;
            Location = new Point(_mainAppSettings.X, _mainAppSettings.Y);
        }
        _windowLocationHelper.EnsureVisible(this);
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

    void RefreshLayoutOfWidgets()
    {
        var x = 0;
        foreach (UserControl widget in PluginManager.Instance.ActiveWidgets)
        {
            int units = Math.Max(1, ((IPluginWidgetControl)widget).WidthInUnits);
            int height = _globalSettings!.WidgetSize;
            int width = units * height;

            widget.Dock = DockStyle.None;
            widget.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            widget.AutoSize = false;

            widget.Bounds = new Rectangle(x, 0, width, height);
            x += width;
        }
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
        foreach (UserControl widget in PluginManager.Instance.ActiveWidgets)
        {
            _contentPanel.Controls.Add(widget);
            ((IPluginWidgetControl)widget).OnRightMouseButtonUp += (_, e) => ShowContextMenu(e);
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
        InvokeConfiguredAction();
    }

    private bool InvokeConfiguredAction()
    {
        var actionDescriptor = PluginManager.Instance.ActionRegistry!.GetAction(_mainAppSettings!.ShortcutAction);
        if (actionDescriptor == null)
        {
            return false;
        }

        var action = default(Action);
        try
        {
            var plugin = PluginManager.Instance.GetPluginById(actionDescriptor.PluginId);
            action = plugin.Actions[actionDescriptor.ActionId];
        }
        catch
        {
            return false;
        }
        action.Invoke();
        return true;
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

    private void OpenConfiguration()
    {
        using var dlg = new ConfigurationWindow(_settingsStore!);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            BackColor = _globalSettings!.HeaderColor;
            RefreshLayout();
            foreach (IPluginWidgetControl widget in PluginManager.Instance.ActiveWidgets)
            {
                widget.SettingsChanged();
            }
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
