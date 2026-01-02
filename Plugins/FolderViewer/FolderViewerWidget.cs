using BassCommon;
using BassCommon.FileSystem;
using PluginContracts;
using PluginCore;
using System.Diagnostics;

namespace FolderViewer;

public class FolderViewerWidget : PluginWidgetControl
{
    private readonly Button _openButton;
    private readonly BlinkHelper _blinker = new();
    private readonly ContextMenuStrip _menu = new();

    public FolderViewerWidget(IPluginContext context) : base(context)
    {
        _openButton = new Button
        {
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0),
            TabStop = false
        };
        _openButton.FlatAppearance.BorderSize = 0;
        _openButton.Click += OpenButton_Click!;
        _openButton.MouseUp += (_, e) => OnMouseUp(e);
        _openButton.Paint += DrawButtonArrow!;

        Controls.Add(_openButton);

        _openButton.BackColor = GlobalSettings.BackgroundColor;
    }

    public override async Task StartAnimation()
    {
        var startColor = _openButton.BackColor;
        await _blinker.BlinkAsync(position =>
        {
            _openButton.BackColor = ColorHelper.GetColorBetween(startColor, Color.White, position);
        });
    }

    public void DisplayFolder()
    {
        _openButton.PerformClick();
    }

    public override void SettingsChanged()
    {
        _menu.Items.Clear();
        _openButton.BackColor = GlobalSettings.BackgroundColor;
        _openButton.Refresh();
    }

    private void OpenButton_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(PluginSettings.Get(SettingKeys.FolderPath)))
        {
            MessageBox.Show("Folder not found.");
            return;
        }

        if (_menu.Items.Count == 0)
        {
            AddMenuItems();
        }

        _menu.Show(_openButton, new Point(
            GetHorizontalAlignment(_openButton, _menu.PreferredSize.Width),
            GetVerticalAlignment(_openButton, _menu.PreferredSize.Height)));
    }

    private void AddMenuItems()
    {
        var items = FolderContentLoader.GetItems(PluginSettings.Get(SettingKeys.FolderPath)!);
        foreach (var item in items)
        {
            item.Click += LeftClickOrEnter;
            item.MouseDown += RightClick;
            _menu.Items.Add(item);
        }
    }

    private void LeftClickOrEnter(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem
            && menuItem.Tag is string path)
        {
            StartProcess(path);
        }
    }

    private void RightClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        if (sender is ToolStripMenuItem menuItem
            && menuItem.Tag is string path
            && menuItem.GetCurrentParent() is ToolStripDropDown)
        {
            ShowWindowsContextMenu(path);
        }
    }

    private void ShowWindowsContextMenu(string path)
    {
        var shellMenu = new ShellContextMenu();
        shellMenu.Show(path, this.ParentForm!.Handle, MousePosition.X, MousePosition.Y);
    }

    private static void StartProcess(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch { }
    }

    private int GetHorizontalAlignment(Button openButton, int preferredWidth)
    {
        var screen = Screen.FromControl(this);
        var screenBounds = screen.WorkingArea;

        var buttonScreenLocation = openButton.PointToScreen(Point.Empty);

        if (buttonScreenLocation.X + preferredWidth > screenBounds.Right)
        {
            return openButton.Width - preferredWidth;
        }

        return 0;
    }

    private int GetVerticalAlignment(Button openButton, int preferredHeight)
    {
        var screen = Screen.FromControl(this);
        var screenBounds = screen.WorkingArea;

        var buttonScreenLocation = openButton.PointToScreen(Point.Empty);

        var yOffset = -preferredHeight;
        if (buttonScreenLocation.Y + yOffset < screenBounds.Top)
        {
            return openButton.Height;
        }

        return yOffset;
    }

    private void DrawButtonArrow(object sender, PaintEventArgs e)
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

        using var brush = new SolidBrush(GlobalSettings.ForegroundColor);
        g.FillPolygon(brush, pts);
    }

    internal void ClearMenu()
    {
        _menu.Items.Clear();
    }
}
