using OneClicker.FileSystem;
using OneClicker.Settings;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using Microsoft.Win32;
using System.Diagnostics;

namespace OneClicker.Forms;

public class MainForm : Form
{
    private readonly IAppSettings _settings;
    private readonly Panel _dragArea;
    private readonly Button _openButton;
    private readonly ContextMenuStrip _popupMenu;
    private readonly ISettingsStorage _settingsIO;
    private bool _isDragging = false;
    private Color _triangleColor;
    private TaskbarHelper _taskbarHelper;

    public MainForm()
    {
        _taskbarHelper = new TaskbarHelper(new ScreenProvider());
        _settings = AppSettings.Instance;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        BackColor = Color.MidnightBlue;
        Size = new Size(20, 20);
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

        _openButton = new Button
        {
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0),
            TabStop = false
        };
        _openButton.FlatAppearance.BorderSize = 0;
        _openButton.Click += OpenButton_Click!;
        _openButton.MouseUp += OpenButton_MouseUp!;
        _openButton.Paint += DrawTriangle!;

        _popupMenu = new ContextMenuStrip();
        _popupMenu.ItemClicked += (s, e) =>
        {
            if (e.ClickedItem!.Tag is string path)
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        };

        Controls.Add(_openButton);
        Controls.Add(_dragArea);

        _settingsIO = new IniSettingsStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"), _settings);//, AppSettings.Instance);
        _settingsIO.Load();
        if (!_settingsIO.FileExists)
        {
            _taskbarHelper.DockAboveTaskbar(this);
            _settings.X = this.Left;
            _settings.Y = this.Top;
        }

        BackColor = _settings.BackColor;
        _openButton.BackColor = _settings.ButtonColor;
        _triangleColor = _settings.TriangleColor; // add field for use in DrawTriangle

        Size = new Size(_settings.Width, _settings.Height);
        Location = new Point(_settings.X, _settings.Y);
        _taskbarHelper.EnsureVisible(this);

        SystemEvents.DisplaySettingsChanged += (s, e) => _taskbarHelper.EnsureVisible(this);
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

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        new TopMostHelper(new Win32WindowPositioner()).HandleMessage(this, ref m);
    }

    private void OpenButton_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(_settings.FolderPath))
        {
            MessageBox.Show("Folder not found.");
            return;
        }

        if (_popupMenu.Items.Count == 0)
        {
            _popupMenu.Items.AddRange(FolderContentLoader.GetItems(_settings.FolderPath).ToArray());
        }
        _popupMenu.Show(_openButton, new Point(0, -_popupMenu.PreferredSize.Height));
    }

    private void OpenButton_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;

        var menu = new ContextMenuStrip();
        menu.Items.Add("Settings", null, (s, a) => ShowSettings());
        menu.Items.Add("Reload Folder", null, (s, a) => _popupMenu.Items.Clear());
        menu.Items.Add("Dock to bottom-right", null, (s, a) => _taskbarHelper.DockAboveTaskbar(this));
        menu.Items.Add("Close", null, (s, a) => Close());
        menu.Show(_openButton, e.Location);
    }

    private void ShowSettings()
    {
        using var dlg = new SettingsForm();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _popupMenu.Items.Clear();
            BackColor = _settings.BackColor;
            _openButton.BackColor = _settings.ButtonColor;
            _triangleColor = _settings.TriangleColor;
            Size = new Size(_settings.Width, _settings.Height);
            _taskbarHelper.EnsureVisible(this);

            _settingsIO.Save();
            Invalidate();
            _openButton.Invalidate();
        }
    }

    private void DrawTriangle(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var w = _openButton.ClientSize.Width;
        var h = _openButton.ClientSize.Height;

        PointF[] pts =
        {
            new PointF(w / 2f, h * 0.25f),
            new PointF(w * 0.25f, h * 0.7f),
            new PointF(w * 0.75f, h * 0.7f)
        };

        using (var brush = new SolidBrush(_triangleColor))
            g.FillPolygon(brush, pts);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _settings.X = Left;
        _settings.Y = Top;
        _settings.Width = Width;
        _settings.Height = Height;
        _settingsIO.Save();
        base.OnFormClosing(e);
    }

    private static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern int SendMessage(nint hWnd, int msg, int wParam, int lParam);
    }
}
