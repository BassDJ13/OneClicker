using PluginContracts;

namespace OneClicker.Forms;

public class AppearanceSettingsPage : UserControl, ISettingsPage
{
    private readonly Button _btnHeaderColor;
    private readonly Button _btnButtonColor;
    private readonly Button _btnTriangleColor;
    private readonly NumericUpDown _numWidgetSize;
    private readonly NumericUpDown _numInactiveOpacity;
    private RadioButton _radioFloating;
    private RadioButton _radioDocked;
    private DockSelectorPanel _dockSelector;
    private Label _labelDock;

    public AppearanceSettingsPage()
    {
        var labelStyle = new Label { Text = "Window Style:", Left = 0, Top = 0, Width = 100 };

        _radioFloating = new RadioButton
        {
            Text = "Floating",
            Left = 110,
            Top = 0,
            AutoSize = true
        };

        _radioDocked = new RadioButton
        {
            Text = "Docked",
            Left = 190,
            Top = 0,
            AutoSize = true
        };

        _radioFloating.CheckedChanged += OnWindowStyleChanged;
        _radioDocked.CheckedChanged += OnWindowStyleChanged;

        _labelDock = new Label { Text = "Dock position:", Left = 0, Top = 27, Width = 100 };

        _dockSelector = new DockSelectorPanel
        {
            Left = 110,
            Top = 27
        };

        var labelBack = new Label { Text = "Header:", Left = 0, Top = 80, Width = 60 };
        _btnHeaderColor = new Button { Left = 60, Top = 77, Width = 22 };

        var labelButton = new Label { Text = "Button:", Left = 0, Top = 106, Width = 60 };
        _btnButtonColor = new Button { Left = 60, Top = 103, Width = 22 };

        var labelTriangle = new Label { Text = "Arrow:", Left = 0, Top = 134, Width = 60 };
        _btnTriangleColor = new Button { Left = 60, Top = 131, Width = 22 };

        var labelWidgetSize = new Label { Text = "WidgetSize:", Left = 110, Top = 80, Width = 102 };
        _numWidgetSize = new NumericUpDown { Left = 212, Top = 77, Width = 60, Minimum = 8, Maximum = 960 };
        var labelInactiveOpacity = new Label { Text = "Inactive opacity:", Left = 110, Top = 109, Width = 102 };
        _numInactiveOpacity = new NumericUpDown { Left = 212, Top = 106, Width = 60, Minimum = 0, Maximum = 100 };


        _btnHeaderColor.Click += (s, e) => PickColor(_btnHeaderColor);
        _btnButtonColor.Click += (s, e) => PickColor(_btnButtonColor);
        _btnTriangleColor.Click += (s, e) => PickColor(_btnTriangleColor);

        Controls.AddRange([
            labelStyle, _radioFloating, _radioDocked,
            _labelDock, _dockSelector,
            labelBack, _btnHeaderColor,
            labelButton, _btnButtonColor,
            labelTriangle, _btnTriangleColor,
            labelWidgetSize, _numWidgetSize,
            labelInactiveOpacity, _numInactiveOpacity]);

        OnWindowStyleChanged(null, EventArgs.Empty);
    }

    private void OnWindowStyleChanged(object? sender, EventArgs e)
    {
        bool isDocked = _radioDocked.Checked;

        _dockSelector.Visible = isDocked;
        _labelDock.Visible = isDocked;
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
        _dockSelector.SetPosition(settings.DockPosition);
    }

    public bool WriteTo(ISettings settings)
    {
        settings.BackColor = _btnHeaderColor.BackColor;
        settings.ButtonColor = _btnButtonColor.BackColor;
        settings.TriangleColor = _btnTriangleColor.BackColor;
        settings.WidgetSize = (int)_numWidgetSize.Value;
        settings.InactiveOpacity = (int)_numInactiveOpacity.Value;
        settings.WindowStyle = _radioDocked.Checked ? WindowStyle.Docked : WindowStyle.Floating;
        settings.DockPosition = _dockSelector.Selected;

        return true;
    }
}
