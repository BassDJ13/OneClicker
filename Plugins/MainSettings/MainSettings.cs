using BassCommon;
using PluginContracts;

namespace MainSettings;

public class MainSettings : UserControl, ISettingsPage
{
    private readonly Button _btnHeaderColor, _btnButtonColor, _btnTriangleColor;
    private readonly NumericUpDown _numWidgetSize, _numInactiveOpacity, _numOffsetX, _numOffsetY;
    private RadioButton _radioFloating, _radioDocked;
    private DockSelectorPanel _dockSelector;
    private Label _labelDock, _labelOffsetX, _labelOffsetY;
    private ShortcutPickerControl _shortcutPicker;
    private CheckBox _startupCheckbox;
    private const string _startupShortcutName = "OneClicker";

    public MainSettings()
    {
        var labelStyle = new Label { Text = "Window Style:", Left = 0, Top = 0, Width = 100 };

        _radioFloating = new RadioButton { Text = "Floating", Left = 110, Top = 0, AutoSize = true };
        _radioDocked = new RadioButton { Text = "Docked", Left = 190, Top = 0, AutoSize = true };
        _radioFloating.CheckedChanged += OnWindowStyleChanged;
        _radioDocked.CheckedChanged += OnWindowStyleChanged;

        _labelDock = new Label { Text = "Dock position:", Left = 0, Top = 27, Width = 100 };

        _dockSelector = new DockSelectorPanel { Left = 110, Top = 27 };
        _dockSelector.SelectedDockChanged += OnDockSelectorValueChanged;

        _labelOffsetX = new Label { Text = "x offset:", Left = 160, Top = 27, Width = 50 };
        _labelOffsetY = new Label { Text = "y offset:", Left = 160, Top = 50, Width = 50 };
        _numOffsetX = new NumericUpDown { Left = 220, Top = 25, Width = 60, Minimum = -9999, Maximum = 9999 };
        _numOffsetY = new NumericUpDown { Left = 220, Top = 48, Width = 60, Minimum = -9999, Maximum = 9999 };

        var labelBack = new Label { Text = "Header:", Left = 0, Top = 85, Width = 60 };
        _btnHeaderColor = new Button { Left = 60, Top = 82, Width = 22 };

        var labelButton = new Label { Text = "Button:", Left = 0, Top = 107, Width = 60 };
        _btnButtonColor = new Button { Left = 60, Top = 104, Width = 22 };

        var labelTriangle = new Label { Text = "Arrow:", Left = 0, Top = 129, Width = 60 };
        _btnTriangleColor = new Button { Left = 60, Top = 126, Width = 22 };

        var labelWidgetSize = new Label { Text = "WidgetSize:", Left = 110, Top = 85, Width = 102 };
        _numWidgetSize = new NumericUpDown { Left = 212, Top = 82, Width = 60, Minimum = 8, Maximum = 960 };
        var labelInactiveOpacity = new Label { Text = "Inactive opacity:", Left = 110, Top = 114, Width = 102 };
        _numInactiveOpacity = new NumericUpDown { Left = 212, Top = 111, Width = 60, Minimum = 0, Maximum = 100 };

        var labelShortcut = new Label { Text = "Focus app:", Left = 0, Top = 156, Width = 70 };
        _shortcutPicker = new ShortcutPickerControl { Left = 75, Top = 154 };

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

        _btnHeaderColor.Click += (s, e) => PickColor(_btnHeaderColor);
        _btnButtonColor.Click += (s, e) => PickColor(_btnButtonColor);
        _btnTriangleColor.Click += (s, e) => PickColor(_btnTriangleColor);

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
        _numOffsetX.Value = 0;
        _numOffsetY.Value = 0;
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
    }

    private void PickColor(Button button)
    {
        using var dlg = new ColorDialog { Color = button.BackColor };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            button.BackColor = dlg.Color;
        }
    }

    public void ReadFrom(ISettings settings)
    {
        _btnHeaderColor.BackColor = settings.BackColor;
        _btnButtonColor.BackColor = settings.ButtonColor;
        _btnTriangleColor.BackColor = settings.TriangleColor;
        _numWidgetSize.Value = Math.Max(8, settings.WidgetSize);
        _numInactiveOpacity.Value = Math.Clamp(settings.InactiveOpacity, 0, 100);
        _radioFloating.Checked = settings.WindowStyle == WindowStyle.Floating;
        _radioDocked.Checked = settings.WindowStyle == WindowStyle.Docked;
        _dockSelector.SelectedDock = settings.DockPosition;
        _numOffsetX.Value = Math.Clamp(settings.DockOffsetX, -9999, 9999);
        _numOffsetY.Value = Math.Clamp(settings.DockOffsetY, -9999, 9999);
        _shortcutPicker.SetShortcutKey(settings.FocusShortcut);
    }

    public bool WriteTo(ISettings settings)
    {
        settings.BackColor = _btnHeaderColor.BackColor;
        settings.ButtonColor = _btnButtonColor.BackColor;
        settings.TriangleColor = _btnTriangleColor.BackColor;
        settings.WidgetSize = (int)_numWidgetSize.Value;
        settings.InactiveOpacity = (int)_numInactiveOpacity.Value;
        settings.WindowStyle = _radioDocked.Checked ? WindowStyle.Docked : WindowStyle.Floating;
        settings.DockPosition = _dockSelector.SelectedDock;
        settings.DockOffsetX = (int)_numOffsetX.Value;
        settings.DockOffsetY = (int)_numOffsetY.Value;
        settings.FocusShortcut = _shortcutPicker.GetShortcutKey();

        return true;
    }
}
