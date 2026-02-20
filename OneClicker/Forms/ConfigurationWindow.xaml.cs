using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using OneClicker.Plugins;
using OneClicker.Settings;
using PluginContracts;
using System.Collections.Generic;

namespace OneClicker.Forms;

public partial class ConfigurationWindow : Window
{
    private readonly ISettingsStore _settingsStore;
    private readonly GlobalSettingsOverlay _globalSettingsOverlay;
    private readonly Dictionary<string, PluginSettingsOverlay> _pluginOverlays = new();
    private readonly PluginManager _pluginManager;
    private string _owner = "BassDJ13";
    private string _repo = "OneClicker";

    public ConfigurationWindow(ISettingsStore settingsStore, PluginManager pluginManager)
    {
        InitializeComponent();
        _settingsStore = settingsStore;
        _pluginManager = pluginManager;
        _globalSettingsOverlay = new GlobalSettingsOverlay(settingsStore);
        // Title = $"OneClicker v{GitHubUpdateChecker.GetVersion()}";
        NavList.SelectionChanged += NavigationIndexChanged;
        SaveButton.Click += SaveButton_Click;
        CancelButton.Click += CancelButton_Click;
        UpdateLink.MouseLeftButtonUp += UpdateLink_MouseLeftButtonUp;
        LoadNavList();
        NavList.SelectedIndex = 0;
        // Loaded += CheckVersion;
    }

    private void LoadNavList()
    {
        NavList.Items.Clear();
        foreach (var item in GetConfigurationMenuItemsSorted())
        {
            NavList.Items.Add(item);
        }
    }

    private IEnumerable<IConfigurationMenuItem> GetConfigurationMenuItemsSorted()
    {
        return _pluginManager.ActivePlugins
            .SelectMany(p => p.ConfigurationMenuItems)
            .Select((item, index) => new
            {
                Item = item,
                Index = index,
                Priority = GetSortPriority(item)
            })
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Item.PluginId, System.StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.Index)
            .Select(x => x.Item);
    }

    private static int GetSortPriority(IConfigurationMenuItem item)
    {
        return item switch
        {
            _ when item.PluginId == "App" && item.Name == "General" => 0,
            _ when item.PluginId == "App" && item.Name == "Appearance" => 1,
            _ when item.PluginId == "App" && item.Name == "Plugins" => 2,
            _ when item.PluginId == "App" && item.Name == "About" => 999,
            _ => 10
        };
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

    // private async void CheckVersion(object? sender, RoutedEventArgs e)
    // {
    //     try
    //     {
    //         var isLatest = await GitHubUpdateChecker.IsLatestVersionAsync(_owner, _repo, GitHubUpdateChecker.GetVersion());
    //         if (!isLatest)
    //         {
    //             UpdateLink.Visibility = Visibility.Visible;
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.WriteLine("Update check failed: " + ex.Message);
    //     }
    // }

    private void NavigationIndexChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (NavList.SelectedItem is IConfigurationMenuItem item)
        {
            LoadConfigurationControl(item);
        }
    }

    private void LoadConfigurationControl(IConfigurationMenuItem configurationMenuItem)
    {
        ContentPanel.Content = null;
        var context = CreateConfigurationContext(configurationMenuItem);
        var configurationControl = configurationMenuItem.CreateConfigurationControl(context);
        if (configurationControl is System.Windows.Controls.Control control)
        {
            ContentPanel.Content = control;
        }
    }

    private IPluginContext CreateConfigurationContext(IConfigurationMenuItem configurationMenuItem)
        => new PluginContext(
            pluginSettings: GetPluginOverlay(configurationMenuItem.PluginId),
            globalSettings: _globalSettingsOverlay,
            actionRegistry: _pluginManager.ActionRegistry!,
            pluginRegistry: _pluginManager.PluginRegistry!);

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        _globalSettingsOverlay.Commit();
        foreach (var overlay in _pluginOverlays.Values)
        {
            overlay.Commit();
        }
        _settingsStore.Save();
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void UpdateLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo($"https://github.com/{_owner}/{_repo}")
        {
            UseShellExecute = true
        });
    }
}
