using OneClicker.Classes;
using OneClicker.Settings;
using System.Diagnostics;

namespace OneClicker.Forms;

public sealed class SettingsForm : Form
{
    private readonly ListBox _navList;
    private readonly Panel _contentPanel;
    private readonly LinkLabel _linkUpdate;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    private string _owner = "BassDJ13";
    private string _repo = "OneClicker";

    private ISettings _localSettings;

    public SettingsForm()
    {
        _localSettings = AppSettings.Instance.Copy();

        Text = $"OneClicker Settings v{GitHubUpdateChecker.GetVersion()}";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 270);
        MaximizeBox = false;
        MinimizeBox = false;

        _navList = new ListBox { Dock = DockStyle.Left, Width = 128 };

        _navList.Items.AddRange(new object[]
        {
            "General",
            "Appearance"
        });

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(8)
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false
        };

        _saveButton = new Button { Text = "Save", Width = 60 };
        _cancelButton = new Button { Text = "Cancel", Width = 60 };

        flow.Controls.Add(_saveButton);
        flow.Controls.Add(_cancelButton);

        bottomPanel.Controls.Add(flow);

        AcceptButton = _saveButton;
        CancelButton = _cancelButton;

        _saveButton.Click += SaveButton_Click;
        _cancelButton.Click += (s, e) => Close();

        _linkUpdate = new LinkLabel { Text = "Update available", Visible = false, Top = 15, Left = 9, AutoSize = true };
        _linkUpdate.LinkClicked += LinkUpdate_LinkClicked;
        bottomPanel.Controls.Add(_linkUpdate);

        Controls.Add(_contentPanel);
        Controls.Add(_navList);
        Controls.Add(bottomPanel);

        _navList.SelectedIndexChanged += NavigationIndexChanged;
        _navList.SelectedIndex = 0;

        Load += CheckVersion;
    }

    private async void CheckVersion(object? sender, EventArgs e)
    {
        Version current = GitHubUpdateChecker.GetVersion();

        bool isLatest = false;
        try
        {
            isLatest = await GitHubUpdateChecker.IsLatestVersionAsync(_owner, _repo, current);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Update check failed: " + ex.Message);
        }

        if (!isLatest)
        {
            _linkUpdate.Visible = true;
        }
    }

    private void NavigationIndexChanged(object? sender, EventArgs e)
    {
        if (_navList.SelectedItem is null)
        {
            return;
        }

        LoadContentPage(_navList.SelectedItem.ToString()!);
    }

    private void LoadContentPage(string pageName)
    {
        SaveContentPageSettings();
        _contentPanel.Controls.Clear();
        UserControl newPage = pageName switch
        {
            "General" => new GeneralSettingsPage(),
            "Appearance" => new AppearanceSettingsPage(),
            _ => throw new ArgumentOutOfRangeException(nameof(pageName))
        };

        (newPage as ISettingsPage)?.ReadFrom(_localSettings);
        newPage.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(newPage);
    }

    private void SaveContentPageSettings()
    {
        if (_contentPanel.Controls.Count > 0 && _contentPanel.Controls[0] is ISettingsPage page)
        {
            page.WriteTo(_localSettings);
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        SaveContentPageSettings();
        AppSettings.Instance.Save(_localSettings);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void LinkUpdate_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start(new ProcessStartInfo($"https://github.com/{_owner}/{_repo}")
        {
            UseShellExecute = true
        });
    }
}
