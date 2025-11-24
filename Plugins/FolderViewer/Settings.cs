using PluginContracts;

namespace FolderViewer;

public class Settings : UserControl, ISettingsPage
{
    private readonly TextBox _textFolder;
    private readonly Button _buttonBrowse;
    private readonly Button _buttonOpen;
    private readonly TextBox _textHotkey;

    public Settings()
    {
        var labelFolder = new Label { Text = "Folder:", Left = 0, Top = 2, Width = 50 };
        _textFolder = new TextBox { Left = 50, Top = 0, Width = 256 };
        _buttonBrowse = new Button { Text = "Browse...", Left = 50, Top = 26, Width = 80 };
        _buttonOpen = new Button { Text = "Open in explorer", Left = 130, Top = 26, Width = 130 };

        var labelHotkey = new Label { Text = "Hotkey:", Left = 0, Top = 62, Width = 50 };
        _textHotkey = new TextBox { Left = 50, Top = 60, Width = 72, Text = "Alt + z", ReadOnly = true };


        _buttonBrowse.Click += (s, e) =>
        {
            using var dlg = new FolderBrowserDialog { SelectedPath = _textFolder.Text };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _textFolder.Text = dlg.SelectedPath;
            }
        };

        _buttonOpen.Click += (s, e) =>
        {
            if (Directory.Exists(_textFolder.Text))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_textFolder.Text) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        Controls.AddRange([labelFolder, _textFolder, _buttonBrowse, _buttonOpen, labelHotkey, _textHotkey]);
    }

    public void ReadFrom(ISettings settings)
    {
        _textFolder.Text = settings.FolderPath;
    }

    public bool WriteTo(ISettings settings)
    {
        if (!Directory.Exists(_textFolder.Text))
        {
            MessageBox.Show("Folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        settings.FolderPath = _textFolder.Text;
        return true;
    }
}
