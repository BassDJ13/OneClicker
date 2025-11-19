using PluginContracts;

namespace OneClicker.Forms;

public class AppearanceSettingsPage : UserControl, ISettingsPage
{
    private readonly Button _btnHeaderColor;
    private readonly Button _btnButtonColor;
    private readonly Button _btnTriangleColor;
    private readonly NumericUpDown _numWidth;
    private readonly NumericUpDown _numHeight;

    public AppearanceSettingsPage()
    {
        var labelBack = new Label { Text = "Header:", Left = 0, Top = 3, Width = 60 };
        _btnHeaderColor = new Button { Left = 60, Top = 0, Width = 22 };
        var labelButton = new Label { Text = "Button:", Left = 0, Top = 29, Width = 60 };
        _btnButtonColor = new Button { Left = 60, Top = 26, Width = 22 };
        var labelTriangle = new Label { Text = "Arrow:", Left = 0, Top = 57, Width = 60 };
        _btnTriangleColor = new Button { Left = 60, Top = 54, Width = 22 };

        var labelWidth = new Label { Text = "Width:", Left = 120, Top = 3, Width = 50 };
        _numWidth = new NumericUpDown { Left = 170, Top = 0, Width = 60, Minimum = 8, Maximum = 960 };
        var labelHeight = new Label { Text = "Height:", Left = 120, Top = 29, Width = 50 };
        _numHeight = new NumericUpDown { Left = 170, Top = 26, Width = 60, Minimum = 12, Maximum = 540 };

        _btnHeaderColor.Click += (s, e) => PickColor(_btnHeaderColor);
        _btnButtonColor.Click += (s, e) => PickColor(_btnButtonColor);
        _btnTriangleColor.Click += (s, e) => PickColor(_btnTriangleColor);

        Controls.AddRange([
            labelBack, _btnHeaderColor,
            labelButton, _btnButtonColor,
            labelTriangle, _btnTriangleColor,
            labelWidth, _numWidth, labelHeight, _numHeight]);
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
        _numWidth.Value = Math.Max(8, settings.Width);
        _numHeight.Value = Math.Max(12, settings.Height);
    }

    public bool WriteTo(ISettings settings)
    {
        settings.BackColor = _btnHeaderColor.BackColor;
        settings.ButtonColor = _btnButtonColor.BackColor;
        settings.TriangleColor = _btnTriangleColor.BackColor;
        settings.Width = (int)_numWidth.Value;
        settings.Height = (int)_numHeight.Value;
        return true;
    }
}
