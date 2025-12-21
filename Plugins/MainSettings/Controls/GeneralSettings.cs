using BassCommon;
using PluginContracts;
using PluginCore;

namespace MainSettings.Controls;

public class GeneralSettings : PluginConfigurationControl
{
    private ShortcutPickerControl _shortcutPicker;
    private CheckBox _startupCheckbox;
    private const string _startupShortcutName = "OneClicker";

    public GeneralSettings(IPluginSettings pluginSettings, IPluginSettings globalSettings) : base(pluginSettings, globalSettings)
    {
        var labelShortcut = new Label { Text = "Focus app:", Left = 0, Top = 2, Width = 70 };
        _shortcutPicker = new ShortcutPickerControl { Left = 75, Top = 0, };
        _shortcutPicker.SetShortcutKey(pluginSettings.Get(SettingKeys.FocusShortcut)!);
        _shortcutPicker.ShortcutChanged += ShortcutPicker_ShortcutChanged;

        var labelStartup = new Label { Text = "Startup:", Left = 0, Top = 28, Width = 70 };
        _startupCheckbox = new CheckBox
        {
            Left = 75,
            Top = 26,
            Width = 260,
            Checked = StartupManager.IsStartupEnabled(_startupShortcutName),
            Text = "Start automatically with Windows"
        };
        _startupCheckbox.CheckedChanged += StartupCheckBox_CheckedChanged;

        Controls.AddRange([
            labelShortcut, _shortcutPicker,
            labelStartup, _startupCheckbox]);
    }

    private void ShortcutPicker_ShortcutChanged(object? sender, EventArgs e)
    {
        PluginSettings.Set(SettingKeys.FocusShortcut, ((ShortcutPickerControl)sender!).GetShortcutKey());
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
}
