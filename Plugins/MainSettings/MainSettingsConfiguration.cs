using BassCommon;
using PluginContracts;
using PluginCore;

namespace MainSettings;

public class MainSettingsConfiguration : PluginConfigurationControl
{
    private readonly Button _btnHeaderColor, _btnButtonColor, _btnTriangleColor;
    private readonly NumericUpDown _numWidgetSize, _numInactiveOpacity, _numOffsetX, _numOffsetY;
    private RadioButton _radioFloating, _radioDocked;
    private DockSelectorPanel _dockSelector;
    private Label _labelDock, _labelOffsetX, _labelOffsetY;
    private ShortcutPickerControl _shortcutPicker;
    private CheckBox _startupCheckbox;
    private const string _startupShortcutName = "OneClicker";

    public MainSettingsConfiguration(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelStyle = new Label { Text = "Window Style:", Left = 0, Top = 0, Width = 100 };

        _radioFloating = new RadioButton { Text = "Floating", Left = 110, Top = 0, AutoSize = true
            , Checked = pluginSettings.Get("WindowStyle") == nameof(WindowStyle.Floating) };
        _radioDocked = new RadioButton { Text = "Docked", Left = 190, Top = 0, AutoSize = true
            , Checked = pluginSettings.Get("WindowStyle") == nameof(WindowStyle.Docked) };
        _radioFloating.CheckedChanged += OnWindowStyleChanged;
        _radioDocked.CheckedChanged += OnWindowStyleChanged;

        _labelDock = new Label { Text = "Dock position:", Left = 0, Top = 27, Width = 100 };

        _dockSelector = new DockSelectorPanel { Left = 110, Top = 27
            , SelectedDock = GetSelectedDockPosition() };
        _dockSelector.SelectedDockChanged += OnDockSelectorValueChanged;

        _labelOffsetX = new Label { Text = "x offset:", Left = 160, Top = 27, Width = 50 };
        _labelOffsetY = new Label { Text = "y offset:", Left = 160, Top = 50, Width = 50 };

        _numOffsetX = new NumericUpDown { Left = 220, Top = 25, Width = 60, Minimum = -9999, Maximum = 9999
            , Value = pluginSettings.GetInt("DockOffsetX") };
        _numOffsetX.ValueChanged += NumOffsetX_ValueChanged;

        _numOffsetY = new NumericUpDown { Left = 220, Top = 48, Width = 60, Minimum = -9999, Maximum = 9999
            , Value = pluginSettings.GetInt("DockOffsetY") };
        _numOffsetY.ValueChanged += NumOffsetY_ValueChanged;

        var labelBack = new Label { Text = "Header:", Left = 0, Top = 85, Width = 60 };
        _btnHeaderColor = new Button { Left = 60, Top = 82, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.BackColor) };

        var labelButton = new Label { Text = "Button:", Left = 0, Top = 107, Width = 60 };
        _btnButtonColor = new Button { Left = 60, Top = 104, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.ButtonColor) };

        var labelTriangle = new Label { Text = "Arrow:", Left = 0, Top = 129, Width = 60 };
        _btnTriangleColor = new Button { Left = 60, Top = 126, Width = 22
            , BackColor = globalSettings.GetColor(GlobalSettingKeys.TriangleColor) };

        var labelWidgetSize = new Label { Text = "WidgetSize:", Left = 110, Top = 85, Width = 102 };
        _numWidgetSize = new NumericUpDown { Left = 212, Top = 82, Width = 60, Minimum = 8, Maximum = 960
            , Value = globalSettings.GetInt(GlobalSettingKeys.WidgetSize) };
        _numWidgetSize.ValueChanged += WidgetSizeChanged;

        var labelInactiveOpacity = new Label { Text = "Inactive opacity:", Left = 110, Top = 114, Width = 102 };
        _numInactiveOpacity = new NumericUpDown { Left = 212, Top = 111, Width = 60, Minimum = 0, Maximum = 100
            , Value = pluginSettings.GetInt("InactiveOpacity") };
        _numInactiveOpacity.ValueChanged += InactiveOpacityChanged;

        var labelShortcut = new Label { Text = "Focus app:", Left = 0, Top = 156, Width = 70 };
        _shortcutPicker = new ShortcutPickerControl { Left = 75, Top = 154, };
        _shortcutPicker.SetShortcutKey(pluginSettings.Get("FocusShortcut")!);
        _shortcutPicker.ShortcutChanged += ShortcutPicker_ShortcutChanged;

        var labelStartup = new Label { Text = "Startup:", Left = 0, Top = 182, Width = 70 };
        _startupCheckbox = new CheckBox
        {
            Left = 75,
            Top = 180,
            Width = 260,
            Checked = StartupManager.IsStartupEnabled(_startupShortcutName),
            Text = "Start automatically with Windows"
        };
        _startupCheckbox.CheckedChanged += StartupCheckBox_CheckedChanged;

        _btnHeaderColor.Click += (s, e) => PickColor(_btnHeaderColor, GlobalSettingKeys.BackColor);
        _btnButtonColor.Click += (s, e) => PickColor(_btnButtonColor, GlobalSettingKeys.ButtonColor);
        _btnTriangleColor.Click += (s, e) => PickColor(_btnTriangleColor, GlobalSettingKeys.TriangleColor);

        Controls.AddRange([
            labelStyle, _radioFloating, _radioDocked,
            _labelDock, _dockSelector,
            _labelOffsetX, _numOffsetX,
            _labelOffsetY, _numOffsetY,
            labelBack, _btnHeaderColor,
            labelButton, _btnButtonColor,
            labelTriangle, _btnTriangleColor,
            labelWidgetSize, _numWidgetSize,
            labelInactiveOpacity, _numInactiveOpacity,
            labelShortcut, _shortcutPicker,
            labelStartup, _startupCheckbox]);

        OnWindowStyleChanged(null, EventArgs.Empty);
    }

    private void ShortcutPicker_ShortcutChanged(object? sender, EventArgs e)
    {
        PluginSettings.Set("FocusShortcut", ((ShortcutPickerControl)sender!).GetShortcutKey());
    }

    private void NumOffsetY_ValueChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt("DockOffsetY", (int)((NumericUpDown)sender!).Value);
    }

    private void NumOffsetX_ValueChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt("DockOffsetX", (int)((NumericUpDown)sender!).Value);
    }

    private void WidgetSizeChanged(object? sender, EventArgs e)
    {
        GlobalSettings.SetInt("WidgetSize", (int)((NumericUpDown)sender!).Value);
    }

    private void InactiveOpacityChanged(object? sender, EventArgs e)
    {
        PluginSettings.SetInt("InactiveOpacity", (int)((NumericUpDown)sender!).Value);
    }

    private DockPosition GetSelectedDockPosition()
    {
        var name = PluginSettings.Get("DockPosition");
        if (Enum.TryParse(name, out DockPosition myStatus))
        {
            return myStatus;
        }
        return DockPosition.BottomRight;
    }

    private void StartupCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (_startupCheckbox.Checked)
        {
            if (!StartupManager.EnableStartup(_startupShortcutName))
            {
                _startupCheckbox.CheckedChanged -= StartupCheckBox_CheckedChanged;
                _startupCheckbox.Checked = false;
                _startupCheckbox.CheckedChanged += StartupCheckBox_CheckedChanged;
            }
        }
        else
        {
            StartupManager.DisableStartup(_startupShortcutName);
        }
    }

    private void OnDockSelectorValueChanged(object? sender, EventArgs e)
    {
        var newValue = ((DockSelectorPanel)sender!).SelectedDock;
        PluginSettings.Set("DockPosition", newValue.ToString());
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
        PluginSettings.Set("WindowStyle", newValue);
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
