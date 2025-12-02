namespace BassCommon;

public class ShortcutCaptureForm : Form
{
    public Keys CapturedShortcut { get; private set; } = Keys.None;

    public ShortcutCaptureForm()
    {
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        Width = 300;
        Height = 90;
        Text = "Press shortcut...";

        KeyPreview = true;

        Controls.Add(new Label
        {
            Text = "Press a key combination...",
            AutoSize = true,
            Left = 20,
            Top = 20
        });
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        TopMost = true;
        BringToFront();
        Activate();

        BeginInvoke(new Action(() =>
        {
            TopMost = true;
            BringToFront();
            Activate();
            Focus();
        }));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        if (e.KeyCode == Keys.ControlKey ||
            e.KeyCode == Keys.ShiftKey ||
            e.KeyCode == Keys.Menu || //Alt
            e.KeyCode == Keys.LWin ||
            e.KeyCode == Keys.RWin)
        {
            return;
        }

        Keys mods = Keys.None;

        if (e.Control)
        {
            mods |= Keys.Control;
        }

        if (e.Alt)
        {
            mods |= Keys.Alt;
        }

        if (e.Shift)
        {
            mods |= Keys.Shift;
        }

        if ((Control.ModifierKeys & Keys.LWin) != 0 ||
            (Control.ModifierKeys & Keys.RWin) != 0)
        {
            mods |= Keys.LWin;
        }

        CapturedShortcut = e.KeyCode | mods;

        DialogResult = DialogResult.OK;
        Close();
    }
}
