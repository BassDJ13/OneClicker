using BassCommon;
using BassCommon.FileSystem;
using PluginContracts;
using System.Diagnostics;

namespace FolderViewer;

public class Widget : PluginWidgetBase
{
    private readonly IAppSettings _settings;
    private readonly Button _openButton;
    private readonly BlinkHelper _blinker = new BlinkHelper();

    public Widget()
    {
        _settings = AppSettings.Instance;

        _openButton = new Button
        {
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0),
            TabStop = false
        };
        _openButton.FlatAppearance.BorderSize = 0;
        _openButton.Click += OpenButton_Click!;
        _openButton.MouseUp += AppServices.MainWindow.HandleMouseUp!;
        _openButton.Paint += DrawButtonArrow!;

        PopupMenuService.PopupMenu.ItemClicked += (s, e) =>
        {
            if (e.ClickedItem!.Tag is string path)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch { }
            }
        };

        Controls.Add(_openButton);

        _openButton.BackColor = _settings.ButtonColor;
    }

    public override async Task StartAnimation()
    {
        var startColor = _openButton.BackColor;
        await _blinker.BlinkAsync(position =>
        {
            _openButton.BackColor = BassCommon.ColorConverter.Convert(startColor, Color.White, position);
        });
    }

    public override void ExecuteAction()
    {
        _openButton.PerformClick();
    }

    public override void ApplySettings()
    {
        PopupMenuService.PopupMenu.Items.Clear();
        _openButton.BackColor = _settings.ButtonColor;
        _openButton.Invalidate();
    }

    private void OpenButton_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(_settings.FolderPath))
        {
            MessageBox.Show("Folder not found.");
            return;
        }

        var popupMenu = PopupMenuService.PopupMenu;
        if (popupMenu.Items.Count == 0)
        {
            popupMenu.Items.AddRange(FolderContentLoader.GetItems(_settings.FolderPath).ToArray());
        }

        popupMenu.Show(_openButton, new Point(
            GetHorizontalAlignment(_openButton, popupMenu.PreferredSize.Width),
            GetVerticalAlignment(_openButton, popupMenu.PreferredSize.Height)));
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

        using var brush = new SolidBrush(_settings.TriangleColor);
        g.FillPolygon(brush, pts);
    }
}
