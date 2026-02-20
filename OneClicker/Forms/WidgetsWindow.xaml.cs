
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OneClicker.Settings;
using OneClicker.Plugins;
using OneClicker.Settings.Ini;
using OneClicker.WindowBehavior;
using PluginContracts;

namespace OneClicker.Forms;

public partial class WidgetsWindow : Window
{
    private MainAppSettings? _mainAppSettings;
    private GlobalSettings? _globalSettings;
    private ISettingsStore? _settingsStore;
    private WindowLocationHelper? _windowLocationHelper;
    // private GlobalHotkeyHelper? _hotkeyHelper; // TODO: Port hotkey logic to WPF
    private ContextMenu? _contextMenu;
    private readonly PluginManager _pluginManager;
    private const int _dragAreaHeight = 6;
    private const int WM_APP_SHOW = 0x8000 + 1;
    // These fields are mapped to x:Name in XAML
    private Border DragArea => (Border)this.FindName("DragArea");
    private StackPanel ContentPanel => (StackPanel)this.FindName("ContentPanel");

    public WidgetsWindow()
    {
        InitializeComponent();
        Title = "OneClicker";
        // _windowLocationHelper = new WindowLocationHelper(new ScreenProvider()); // TODO: Port window location logic to WPF
        this.WindowStyle = System.Windows.WindowStyle.None;
        Topmost = true;
        ShowInTaskbar = false;

        _settingsStore = new IniSettingsStore(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
        _settingsStore.Load();

        _mainAppSettings = new MainAppSettings(_settingsStore);
        _globalSettings = new GlobalSettings(_settingsStore);
        _pluginManager = new PluginManager(_settingsStore, _globalSettings);

        if (!_settingsStore.FileExists)
        {
            // SetDockedLocation();
            _mainAppSettings.X = (int)Left;
            _mainAppSettings.Y = (int)Top;
        }

        // TransparencyHelper.AttachAutoOpacity(this, _mainAppSettings!.InactiveOpacity / 100f); // Port to WPF if needed

        // DragArea.MouseLeftButtonDown += DragArea_MouseLeftButtonDown; // TODO: Port drag logic to WPF
        // LocationChanged += WidgetsWindow_LocationChanged; // TODO: Port location changed logic to WPF

        var brush = new BrushConverter().ConvertFrom(_globalSettings!.HeaderColor) as SolidColorBrush;
        Background = brush ?? Brushes.MidnightBlue;
        RefreshLayout();

        // Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void RefreshLayout()
    {
        var widgetSize = _globalSettings!.WidgetSize;
        // var headerHeight = _mainAppSettings!.WindowStyle == WindowStyle.Floating ? _dragAreaHeight : 0;
        var headerHeight = 0;

        var appWidth = widgetSize * Math.Max(1, _pluginManager.WidthOfWidgetsInUnits());
        var appHeight = widgetSize + headerHeight;
        Width = appWidth;
        Height = appHeight;

        // ApplyWindowStyle(); // TODO: Port window style logic to WPF
        // TODO: RefreshLayoutOfWidgets and Blink
    }

    // private void ApplyWindowStyle()
    // {
    //     if (_mainAppSettings!.WindowStyle == WindowStyle.Docked)
    //     {
    //         DragArea.Visibility = Visibility.Collapsed;
    //         SetDockedLocation();
    //     }
    //     else
    //     {
    //         DragArea.Visibility = Visibility.Visible;
    //         Left = _mainAppSettings.X;
    //         Top = _mainAppSettings.Y;
    //     }
    //     _windowLocationHelper.EnsureVisible(this);
    // }

    // private void SetDockedLocation()
    // {
    //     var wa = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle).WorkingArea;
    //     var pos = _windowLocationHelper.GetDockedPosition(wa, new System.Drawing.Size((int)Width, (int)Height), _mainAppSettings!.DockPosition, _mainAppSettings!.DockOffsetX, _mainAppSettings!.DockOffsetY);
    //     Left = pos.X;
    //     Top = pos.Y;
    // }

    // private void DragArea_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    // {
    //     NativeMethods.ReleaseCapture();
    //     NativeMethods.SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, 0xA1, 0x2, 0);
    // }

    // private void WidgetsWindow_LocationChanged(object? sender, EventArgs e)
    // {
    //     _windowLocationHelper.KeepInWorkArea(this);
    // }
    public void Blink() => _ = BlinkAsync();

    private async Task BlinkAsync()
    {
        // TODO: Port plugin widget animation logic to WPF
        await Task.CompletedTask;
    }

    private void RefreshLayoutOfWidgets()
    {
        ContentPanel.Children.Clear();
        // TODO: Port plugin widget display logic to WPF
        // Placeholder: Add a TextBlock for each widget
        foreach (var widget in _pluginManager.ActiveWidgets)
        {
            var tb = new TextBlock { Text = widget.GetType().Name, Margin = new Thickness(4) };
            ContentPanel.Children.Add(tb);
        }
    }

    // private void ShowContextMenuWpf(System.Windows.Point position)
    // {
    //     if (_contextMenu != null)
    //     {
    //         _contextMenu.IsOpen = true;
    //         _contextMenu.PlacementTarget = this;
    //         _contextMenu.HorizontalOffset = position.X;
    //         _contextMenu.VerticalOffset = position.Y;
    //         return;
    //     }
    //     _contextMenu = new ContextMenu();
    //     // ContextMenuService.CreateMenuItemsForPlugins(_contextMenu, _pluginManager.ActivePlugins);
    //     var configItem = new MenuItem { Header = "Configuration" };
    //     // configItem.Click += (s, e) => OpenConfiguration();
    //     _contextMenu.Items.Add(configItem);
    //     var closeItem = new MenuItem { Header = "Close program" };
    //     closeItem.Click += (s, e) => Close();
    //     _contextMenu.Items.Add(closeItem);
    //     _contextMenu.PlacementTarget = this;
    //     _contextMenu.HorizontalOffset = position.X;
    //     _contextMenu.VerticalOffset = position.Y;
    //     _contextMenu.IsOpen = true;
    // }
}
