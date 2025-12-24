using PluginContracts;
using System.ComponentModel;

namespace MainSettings.Controls;

public sealed class PluginActionComboBox : ComboBox
{
    private const string NoneDisplayText = "<None>";

    public event EventHandler? SelectedActionChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PluginActionDescriptor? SelectedAction
    {
        get => SelectedItem as PluginActionDescriptor;
        set
        {
            if (value == null)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedItem = value;
            }
        }
    }

    public PluginActionComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        DisplayMember = nameof(PluginActionDescriptor.DisplayName);

        SelectedIndexChanged += (_, _) =>
        {
            SelectedActionChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    public void LoadActions(IActionRegistry? registry)
    {
        Items.Clear();
        Items.Add(new PluginActionDescriptor(string.Empty, string.Empty, NoneDisplayText));

        if (registry == null)
        {
            SelectedIndex = 0;
            return;
        }

        var sortedActions = registry
            .GetAllActions()
            .OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase);

        foreach (var action in sortedActions)
        {
            Items.Add(action);
        }

        SelectedIndex = 0;
    }
}
