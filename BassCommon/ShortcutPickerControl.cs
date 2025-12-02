using BassCommon.Classes;

namespace BassCommon;

public class ShortcutPickerControl : UserControl
{
    private readonly TextBox _txtShortcut;
    private readonly Button _btnSet;
    private readonly Label _lblWarning;

    public event EventHandler? ShortcutChanged;

    public Keys Shortcut { get; private set; } = Keys.None;

    public ShortcutPickerControl()
    {
        Height = 30;
        Width = 200;

        _txtShortcut = new TextBox
        {
            Left = 0,
            Top = 0,
            Width = 110,
            ReadOnly = true,
            TabStop = false
        };

        _btnSet = new Button
        {
            Left = 112,
            Top = 0,
            Width = 40,
            Text = "Set"
        };
        _btnSet.Click += BtnSet_Click;

        _lblWarning = new Label
        {
            Left = 0,
            Top = 28,
            ForeColor = System.Drawing.Color.DarkRed,
            AutoSize = true,
            Visible = false
        };

        Controls.Add(_txtShortcut);
        Controls.Add(_btnSet);
        Controls.Add(_lblWarning);
    }

    private void BtnSet_Click(object? sender, EventArgs e)
    {
        using var popup = new ShortcutCaptureForm();
        if (popup.ShowDialog(this) == DialogResult.OK)
        {
            Shortcut = popup.CapturedShortcut;
            _txtShortcut.Text = KeyParser.ToSettingString(Shortcut);

            EvaluateConflict();
            ShortcutChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void EvaluateConflict()
    {
        if (GlobalHotkeyRegistry.IsTaken(Shortcut))
        {
            _lblWarning.Text = "Shortcut is already in use.";
            _lblWarning.Visible = true;
        }
        else
        {
            _lblWarning.Visible = false;
        }
    }

    public void SetShortcutKey(string shortcut)
    {
        Shortcut = KeyParser.FromSettingString(shortcut);
        _txtShortcut.Text = shortcut;
        EvaluateConflict();
    }

    public string GetShortcutKey()
    {
        return _txtShortcut.Text;
    }
}
