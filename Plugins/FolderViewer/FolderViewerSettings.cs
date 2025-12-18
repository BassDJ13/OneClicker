using PluginContracts;
using PluginCore;

namespace FolderViewer;

public class FolderViewerSettings : PluginSettingsControlBase
{
    private readonly TextBox _textFolder;
    private readonly Button _buttonBrowse;
    private readonly Button _buttonOpen;

    public FolderViewerSettings(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelFolder = new Label { Text = "Folder:", Left = 0, Top = 2, Width = 50 };
        _textFolder = new TextBox { Left = 50, Top = 0, Width = 256, Text = pluginSettings.Get("FolderPath") };
        _textFolder.TextChanged += TextFolder_TextChanged;
        _buttonBrowse = new Button { Text = "Browse...", Left = 50, Top = 26, Width = 80 };
        _buttonOpen = new Button { Text = "Open in explorer", Left = 130, Top = 26, Width = 130 };

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

        Controls.AddRange([labelFolder, _textFolder, _buttonBrowse, _buttonOpen]);
    }

    private void TextFolder_TextChanged(object? sender, EventArgs e)
    {
        PluginSettings.Set("FolderPath", ((TextBox)sender!).Text);
    }
}