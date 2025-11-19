using Microsoft.Win32;
using OneClicker.Classes;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using PluginContracts;

namespace OneClicker.Forms;

public class MainForm : Form, IMainWindow
{
    private readonly IAppSettings _settings;
    private readonly Panel _dragArea;
    private readonly Panel _contentPanel;
    private readonly ISettingsStorage _settingsIO;
    private bool _isDragging = false;
    private TaskbarHelper _taskbarHelper;
    private GlobalHotkeyHelper? _hotkeyHelper;
    private int _widgetWidth = 0;
    private int _widgetHeight = 0;

    private void Blink() => _ = BlinkAsync();

    private async Task BlinkAsync()
    {
        var tasks = _contentPanel.Controls
            .OfType<IPluginWidget>()
            .Select(widget => widget.StartAnimation())
            .ToList();
        await Task.WhenAll(tasks);
    }

    public MainForm()
    {
        PluginManager.Initialize(this);
        Text = "OneClicker";
        _taskbarHelper = new TaskbarHelper(new ScreenProvider());
        _settings = AppSettings.Instance;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        BackColor = Color.MidnightBlue;
        TransparencyHelper.AttachAutoOpacity(this);

        _dragArea = new Panel
        {
            Dock = DockStyle.Top,
            Height = 6,
            Cursor = Cursors.SizeAll,
            BackColor = Color.Transparent
        };
        _dragArea.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        };
        _dragArea.MouseUp += (s, e) =>
        {
            if (_isDragging)
            {
                _isDragging = false;
                _taskbarHelper.KeepInWorkArea(this);
                _settings.X = Left;
                _settings.Y = Top;
                _settingsIO!.Save();
            }
        };
        _dragArea.MouseMove += (s, e) =>
        {
            if (_isDragging)
            {
                _taskbarHelper.KeepInWorkArea(this);
            }
        };

        _contentPanel = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0)
        };

        Controls.Add(_contentPanel);
        Controls.Add(_dragArea);

        _settingsIO = new IniSettingsStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"), _settings);
        _settingsIO.Load();
        if (!_settingsIO.FileExists)
        {
            _taskbarHelper.DockAboveTaskbar(this);
            _settings.X = this.Left;
            _settings.Y = this.Top;
        }

        BackColor = _settings.BackColor;
        RefreshSize();
        Location = new Point(_settings.X, _settings.Y);
        _taskbarHelper.EnsureVisible(this);

        SystemEvents.DisplaySettingsChanged += (s, e) => _taskbarHelper.EnsureVisible(this);
    }

    private void RefreshSize()
    {
        _widgetWidth = _settings.Width;
        _widgetHeight = _settings.Height;
        Size = new Size(_widgetWidth * PluginManager.Instance.ActivePlugins.Count, _widgetHeight);
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
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            var widgetControl = plugin.WidgetControl;
            widgetControl.Dock = DockStyle.Fill; //todo: stack widgets horizontal
            _contentPanel.Controls.Add(widgetControl);
            widgetControl.ApplySettings();
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
        _hotkeyHelper = new GlobalHotkeyHelper(Handle, OnGlobalHotkeyPressed);
    }

    private void OnGlobalHotkeyPressed()
    {
        ShowAndActivate();
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            plugin.WidgetControl.ExecuteAction();
        }
    }

    private void ShowAndActivate()
    {
        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
        }
        Activate();
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
            Blink();
            return;
        }

        base.WndProc(ref m);
        new TopMostHelper(new Win32WindowPositioner()).HandleMessage(this, ref m);
    }

    public void HandleMouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        var menu = new ContextMenuStrip();
        menu.Items.Add("Settings", null, (s, a) => ShowSettings());
        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            if (plugin.WidgetControl is IPluginContextMenu pluginContextMenu)
            {
                menu.Items.Add(pluginContextMenu.MainMenuName);
                (menu.Items[menu.Items.Count - 1] as ToolStripMenuItem)!.DropDownItems.AddRange(pluginContextMenu.SubMenuItems);
            }
        }
        menu.Items.Add("Dock to bottom-right", null, (s, a) =>
        {
            _taskbarHelper.DockAboveTaskbar(this);
            Blink();
        });
        menu.Items.Add("Close", null, (s, a) => Close());
        menu.Show(this, e.Location);
    }

    private void ShowSettings()
    {
        using var dlg = new SettingsForm();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            BackColor = _settings.BackColor;
            RefreshSize();
            _taskbarHelper.EnsureVisible(this);
            _settingsIO.Save();
            Invalidate();

            foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
            {
                plugin.WidgetControl.ApplySettings();
            }
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _settings.X = Left;
        _settings.Y = Top;
        _settings.Width = _widgetWidth;
        _settings.Height = _widgetHeight;
        _settingsIO.Save();
        base.OnFormClosing(e);
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
