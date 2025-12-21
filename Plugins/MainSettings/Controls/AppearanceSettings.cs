using PluginContracts;
using PluginCore;

namespace MainSettings.Controls;

public class AppearanceSettings : PluginConfigurationControl
{
    private readonly Button _btnHeaderColor, _btnBackgroundColor, _btnForeGroundColor;
    private readonly NumericUpDown _numWidgetSize, _numInactiveOpacity, _numOffsetX, _numOffsetY;
    private RadioButton _radioFloating, _radioDocked;
    private DockSelectorPanel _dockSelector;
    private Label _labelDock, _labelOffsetX, _labelOffsetY;

    public AppearanceSettings(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelStyle = new Label { Text = "Window Style:", Left = 0, Top = 0, Width = 100 };

        _radioFloating = new RadioButton { Text = "Floating", Left = 110, Top = 0, AutoSize = true
            , Checked = pluginSettings.Get(SettingKeys.WindowStyle) == nameof(WindowStyle.Floating) };
        _radioDocked = new RadioButton { Text = "Docked", Left = 190, Top = 0, AutoSize = true
            , Checked = pluginSettings.Get(SettingKeys.WindowStyle) == nameof(WindowStyle.Docked) };
        _radioFloating.CheckedChanged += OnWindowStyleChanged;
        _radioDocked.CheckedChanged += OnWindowStyleChanged;

        _labelDock = new Label { Text = "Dock position:", Left = 0, Top = 27, Width = 100 };

        _dockSelector = new DockSelectorPanel { Left = 110, Top = 27
            , SelectedDock = GetSelectedDockPosition() };
        _dockSelector.SelectedDockChanged += OnDockSelectorValueChanged;

        _labelOffsetX = new Label { Text = "x offset:", Left = 160, Top = 27, Width = 50 };
        _labelOffsetY = new Label { Text = "y offset:", Left = 160, Top = 50, Width = 50 };

        _numOffsetX = new NumericUpDown { Left = 220, Top = 25, Width = 60, Minimum = -9999, Maximum = 9999
            , Value = pluginSettings.GetInt(SettingKeys.DockOffsetX) };
        _numOffsetX.ValueChanged += NumOffsetX_ValueChanged;

        _numOffsetY = new NumericUpDown { Left = 220, Top = 48, Width = 60, Minimum = -9999, Maximum = 9999
            , Value = pluginSettings.GetInt(SettingKeys.DockOffsetY) };
        _numOffsetY.ValueChanged += NumOffsetY_ValueChanged;

        var labelBack = new Label { Text = "Header:", Left = 0, Top = 85, Width = 74 };
        _btnHeaderColor = new Button { Left = 74, Top = 82, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.HeaderColor) };

        var labelButton = new Label { Text = "Background:", Left = 0, Top = 107, Width = 74 };
        _btnBackgroundColor = new Button { Left = 74, Top = 104, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.BackgroundColor) };

        var labelTriangle = new Label { Text = "Foreground:", Left = 0, Top = 129, Width = 74 };
        _btnForeGroundColor = new Button { Left = 74, Top = 126, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.ForegroundColor) };

        var labelWidgetSize = new Label { Text = "WidgetSize:", Left = 120, Top = 85, Width = 100 };
        _numWidgetSize = new NumericUpDown { Left = 220, Top = 82, Width = 60, Minimum = 8, Maximum = 960
            , Value = globalSettings.GetInt(GlobalSettingKeys.WidgetSize) };
        _numWidgetSize.ValueChanged += WidgetSizeChanged;

        var labelInactiveOpacity = new Label { Text = "Inactive opacity:", Left = 120, Top = 114, Width = 100 };
        _numInactiveOpacity = new NumericUpDown { Left = 220, Top = 111, Width = 60, Minimum = 0, Maximum = 100
            , Value = pluginSettings.GetInt(SettingKeys.InactiveOpacity) };
        _numInactiveOpacity.ValueChanged += InactiveOpacityChanged;

        _btnHeaderColor.Click += (s, e) => PickColor(_btnHeaderColor, GlobalSettingKeys.HeaderColor);
        _btnBackgroundColor.Click += (s, e) => PickColor(_btnBackgroundColor, GlobalSettingKeys.BackgroundColor);
        _btnForeGroundColor.Click += (s, e) => PickColor(_btnForeGroundColor, GlobalSettingKeys.ForegroundColor);

        Controls.AddRange([
            labelStyle, _radioFloating, _radioDocked,
            _labelDock, _dockSelector,
            _labelOffsetX, _numOffsetX,
            _labelOffsetY, _numOffsetY,
            labelBack, _btnHeaderColor,
            labelButton, _btnBackgroundColor,
            labelTriangle, _btnForeGroundColor,
            labelWidgetSize, _numWidgetSize,
            labelInactiveOpacity, _numInactiveOpacity]);

        OnWindowStyleChanged(null, EventArgs.Empty);
    }

    private void NumOffsetY_ValueChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt(
            SettingKeys.DockOffsetY, 
            (int)((NumericUpDown)sender!).Value);
    }

    private void NumOffsetX_ValueChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt(
            SettingKeys.DockOffsetX, 
            (int)((NumericUpDown)sender!).Value);
    }

    private void WidgetSizeChanged(object? sender, EventArgs e)
    {
        GlobalSettings.SetInt(
            GlobalSettingKeys.WidgetSize, 
            (int)((NumericUpDown)sender!).Value);
    }

    private void InactiveOpacityChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt(
            SettingKeys.InactiveOpacity,
            (int)((NumericUpDown)sender!).Value);
    }

    private DockPosition GetSelectedDockPosition()
    {
        var name = PluginSettings.Get(SettingKeys.DockPosition);
        if (Enum.TryParse(name, out DockPosition myStatus))
        {
            return myStatus;
        }
        return DockPosition.BottomRight;
    }

    private void OnDockSelectorValueChanged(object? sender, EventArgs e)
    {
        var newValue = ((DockSelectorPanel)sender!).SelectedDock;
        PluginSettings.Set(
            SettingKeys.DockPosition, 
            newValue.ToString());
    }

    private void OnWindowStyleChanged(object? sender, EventArgs e)
    {
        bool isDocked = _radioDocked.Checked;

        _dockSelector.Visible = isDocked;
        _labelDock.Visible = isDocked;
        _labelOffsetX.Visible = isDocked;
        _numOffsetX.Visible = isDocked;
        _labelOffsetY.Visible = isDocked;
        _numOffsetY.Visible = isDocked;

        var newValue = isDocked
            ? nameof(WindowStyle.Docked)
            : nameof(WindowStyle.Floating);
        
        PluginSettings.Set(
            SettingKeys.WindowStyle,
            newValue);
    }

    private void PickColor(Button button, string settingName)
    {
        using var dlg = new ColorDialog { Color = button.BackColor };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            button.BackColor = dlg.Color;
            GlobalSettings.SetColor(settingName, dlg.Color);
        }
    }
}
