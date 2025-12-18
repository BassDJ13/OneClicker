using BassCommon.Classes;
using OneClicker.Plugins;
using OneClicker.Settings;
using PluginContracts;
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

    private readonly ISettingsStore _settingsStore;
    private readonly GlobalSettingsOverlay _globalSettingsOverlay;
    private readonly Dictionary<string, PluginSettingsOverlay> _pluginOverlays = new();

    public SettingsForm(ISettingsStore settingsStore)
    {
        _settingsStore = settingsStore;
        _globalSettingsOverlay = new GlobalSettingsOverlay(settingsStore);

        Text = $"OneClicker Settings v{GitHubUpdateChecker.GetVersion()}";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 270);
        MaximizeBox = false;
        MinimizeBox = false;

        _navList = new ListBox { Dock = DockStyle.Left, Width = 128, DisplayMember = "Name" };

        foreach (IPlugin plugin in PluginManager.Instance.ActivePlugins)
        {
            foreach (ISettingsItem settingsItem in plugin.SettingsItems)
            {
                _navList.Items.Add(settingsItem);
            }
        }

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
        _cancelButton.Click += CancelButton_Click;

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

    private PluginSettingsOverlay GetPluginOverlay(string pluginId)
    {
        if (!_pluginOverlays.TryGetValue(pluginId, out var overlay))
        {
            overlay = new PluginSettingsOverlay(pluginId, _settingsStore);
            _pluginOverlays.Add(pluginId, overlay);
        }

        return overlay;
    }

    private async void CheckVersion(object? sender, EventArgs e)
    {
        try
        {
            var isLatest = await GitHubUpdateChecker.IsLatestVersionAsync(_owner, _repo, GitHubUpdateChecker.GetVersion());
            if (!isLatest)
            {
                _linkUpdate.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Update check failed: " + ex.Message);
        }
    }

    private void NavigationIndexChanged(object? sender, EventArgs e)
    {
        if (_navList.SelectedItem is null)
        {
            return;
        }

        LoadContentPage((ISettingsItem)_navList.SelectedItem);
    }

    private void LoadContentPage(ISettingsItem settingsItem)
    {
        _contentPanel.Controls.Clear();

        var settingsControl = settingsItem.CreateContent(GetPluginOverlay(settingsItem.PluginId), _globalSettingsOverlay);
        if (settingsControl == null)
        {
            return;
        }

        settingsControl.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add((Control)settingsControl);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        _globalSettingsOverlay.Commit();

        foreach (var overlay in _pluginOverlays.Values)
        {
            overlay.Commit();
        }

        _settingsStore.Save();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
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
