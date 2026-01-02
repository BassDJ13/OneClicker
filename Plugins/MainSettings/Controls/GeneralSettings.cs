using BassCommon;
using PluginContracts;
using PluginCore;

namespace MainSettings.Controls;

public class GeneralSettings : PluginConfigurationControl
{
    private ShortcutPickerControl _shortcutPicker;
    private CheckBox _startupCheckbox;
    private const string _startupShortcutName = "OneClicker";
    private readonly IActionRegistry _allPluginsActions;

    public GeneralSettings(IPluginContext pluginContext) : base(pluginContext)
    {
        var rowHeight = 26;
        var offsetX = 75;
        _allPluginsActions = ((IHostPluginContext)pluginContext).ActionRegistry;

        var labelShortcut = new Label { Text = "Focus app:", Left = 0, Top = 2, Width = 70 };
        _shortcutPicker = new ShortcutPickerControl { Left = offsetX, Top = 0 };

        var labelAction = new Label { Text = "Execute action:", Left = 0, Top = rowHeight+2, Width = 70 };
        var actionCombobox = new PluginActionComboBox { Left = offsetX, Top = rowHeight, Width = 200 };

        var labelStartup = new Label { Text = "Startup:", Left = 0, Top = rowHeight*2 + 2, Width = 70 };
        _startupCheckbox = new CheckBox { Left = offsetX, Top = rowHeight*2, Width = 260,
            Checked = StartupManager.IsStartupEnabled(_startupShortcutName),
            Text = "Start automatically with Windows"
        };

        Controls.AddRange([
            labelShortcut, _shortcutPicker,
            labelStartup, _startupCheckbox,
            labelAction, actionCombobox]);

        _startupCheckbox.CheckedChanged += StartupCheckBox_CheckedChanged;
        _shortcutPicker.SetShortcutKey(PluginSettings.Get(SettingKeys.FocusShortcut)!);
        _shortcutPicker.ShortcutChanged += ShortcutPicker_ShortcutChanged;
        actionCombobox.LoadActions(_allPluginsActions); //todo: row below depends on this, fix it
        actionCombobox.SelectedAction = _allPluginsActions.GetAction(PluginSettings.Get(SettingKeys.ShortcutAction)!);
        actionCombobox.SelectedActionChanged += ActionCombobox_SelectedActionChanged;
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

    private void ActionCombobox_SelectedActionChanged(object? sender, EventArgs e)
    {
        var selected = ((PluginActionComboBox)sender!).SelectedAction;
        PluginSettings.Set(SettingKeys.ShortcutAction, selected!.UniqueKey);
    }
}
