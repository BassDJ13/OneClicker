using OneClicker.Classes;
using OneClicker.Settings;

namespace OneClicker.Forms;

public class SettingsForm : Form
{
    private LinkLabel _linkUpdate;
    private string _owner = "BassDJ13";
    private string _repo = "OneClicker";

    public SettingsForm()
    {
        Load += SettingsForm_Load;

        var settings = AppSettings.Instance;
        Text = $"OneClicker Settings v{ParseVersionSafe(Application.ProductVersion)}";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(368, 232);
        MaximizeBox = false;
        MinimizeBox = false;

        // --- Folder ---
        var labelFolder = new Label { Text = "Folder:", Left = 10, Top = 20, Width = 50 };
        var textFolder = new TextBox { Left = 70, Top = 18, Width = 220, Text = settings.FolderPath };
        var buttonBrowse = new Button { Text = "Browse...", Left = 290, Top = 18, Width = 65 };
        var buttonOpen = new Button { Text = "Open in explorer", Left = 70, Top = 44, Width = 110 };

        buttonBrowse.Click += (s, e) =>
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select folder to display",
                SelectedPath = textFolder.Text
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textFolder.Text = dialog.SelectedPath;
            }
        };

        buttonOpen.Click += (s, e) =>
        {
            if (Directory.Exists(textFolder.Text))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(textFolder.Text) { UseShellExecute = true });
            }
        };

        // --- Hotkey ---
        var labelHotkey = new Label { Text = "Hotkey:", Left = 10, Top = 72, Width = 50 };
        var textHotkey = new TextBox { Left = 70, Top = 70, Width = 50, Text = "Alt + z", ReadOnly = true };

        // --- Colors ---
        var labelBack = new Label { Text = "Header:", Left = 10, Top = 110, Width = 60 };
        var labelButton = new Label { Text = "Button:", Left = 10, Top = 135, Width = 60 };
        var labelTriangle = new Label { Text = "Arrow:", Left = 10, Top = 160, Width = 60 };

        var buttonHeaderColor = new Button { Left = 70, Top = 105, Width = 22, BackColor = settings.BackColor };
        var buttonBackgroundColor = new Button { Left = 70, Top = 130, Width = 22, BackColor = settings.ButtonColor };
        var buttonForegroundColor = new Button { Left = 70, Top = 155, Width = 22, BackColor = settings.TriangleColor };

        buttonHeaderColor.Click += (s, e) => PickColor(buttonHeaderColor);
        buttonBackgroundColor.Click += (s, e) => PickColor(buttonBackgroundColor);
        buttonForegroundColor.Click += (s, e) => PickColor(buttonForegroundColor);

        // --- Size ---
        var labelWidth = new Label { Text = "Width:", Left = 175, Top = 110, Width = 50 };
        var numericWidth = new NumericUpDown { Left = 230, Top = 107, Width = 60, Minimum = 8, Maximum = 960, Value = Math.Max(8, settings.Width) };
        var labelHeight = new Label { Text = "Height:", Left = 175, Top = 135, Width = 50 };
        var numericHeight = new NumericUpDown { Left = 230, Top = 132, Width = 60, Minimum = 12, Maximum = 540, Value = Math.Max(12, settings.Height) };

        _linkUpdate = new LinkLabel { Text = "Update is available", Left = 10, Top = 195, Width = 200 };
        _linkUpdate.LinkClicked += LinkUpdate_LinkClicked;
        _linkUpdate.Visible = false;

        // --- Save/Cancel ---
        var saveButton = new Button { Text = "Save", Left = 226, Width = 60, Top = 195, DialogResult = DialogResult.OK };
        var cancelButton = new Button { Text = "Cancel", Left = 296, Width = 60, Top = 195, DialogResult = DialogResult.Cancel };
        AcceptButton = saveButton;
        CancelButton = cancelButton;

        Controls.AddRange(new Control[]
        {
            labelFolder, textFolder, buttonBrowse, buttonOpen,
            labelHotkey, textHotkey,
            labelBack, labelButton, labelTriangle,
            buttonHeaderColor, buttonBackgroundColor, buttonForegroundColor,
            labelWidth, numericWidth,
            labelHeight, numericHeight,
            _linkUpdate,
            saveButton, cancelButton
        });

        saveButton.Click += (s, e) =>
        {
            if (!Directory.Exists(textFolder.Text))
            {
                MessageBox.Show("Selected folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            settings.FolderPath = textFolder.Text;
            settings.BackColor = buttonHeaderColor.BackColor;
            settings.ButtonColor = buttonBackgroundColor.BackColor;
            settings.TriangleColor = buttonForegroundColor.BackColor;
            settings.Width = (int)numericWidth.Value;
            settings.Height = (int)numericHeight.Value;
        };
    }

    private async void SettingsForm_Load(object? sender, EventArgs e)
    {
        var currentVersion = ParseVersionSafe(Application.ProductVersion);
        bool isLatest = await GitHubUpdateChecker.IsLatestVersionAsync(_owner, _repo, currentVersion);

        if (!isLatest)
        {
            _linkUpdate.Visible = true;
        }
    }

    private static Version ParseVersionSafe(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
            return new Version(0, 0, 0, 0);

        // Trim build metadata like "+commitsha"
        var clean = versionString.Split('+')[0];
        if (Version.TryParse(clean, out var version))
            return version;

        return new Version(0, 0, 0, 0);
    }

    private static void PickColor(Button button)
    {
        var cd = new ColorDialog
        {
            Color = button.BackColor
        };

        if (cd.ShowDialog() == DialogResult.OK)
        {
            button.BackColor = cd.Color;
        }
    }

    private void LinkUpdate_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = $"https://github.com/{_owner}/{_repo}",
            UseShellExecute = true
        });
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _linkUpdate.LinkClicked -= LinkUpdate_LinkClicked;
        _linkUpdate?.Dispose();
        base.OnFormClosed(e);
    }
}
