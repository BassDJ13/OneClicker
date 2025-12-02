using PluginContracts;
using System.ComponentModel;

namespace MainSettings;

public class DockSelectorPanel : Panel
{
    private readonly Dictionary<DockPosition, Button> _buttons = new();
    
    private DockPosition _selectedDock = DockPosition.BottomRight;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DockPosition SelectedDock
    {
        get => _selectedDock;
        set
        {
            if (_selectedDock == value)
                return;

            _selectedDock = value;
            OnSelectedDockChanged();
        }
    }

    public event EventHandler? SelectedDockChanged;

    private readonly Button _tl;
    private readonly Button _tc;
    private readonly Button _tr;
    private readonly Button _ml;
    private readonly Button _mr;
    private readonly Button _bl;
    private readonly Button _bc;
    private readonly Button _br;

    public DockSelectorPanel()
    {
        Width = 40;
        Height = 40;

        _tl = CreateButton(DockPosition.TopLeft, 0, 0);
        _tc = CreateButton(DockPosition.Top, 13, 0);
        _tr = CreateButton(DockPosition.TopRight, 26, 0);

        _ml = CreateButton(DockPosition.Left, 0, 13);
        _mr = CreateButton(DockPosition.Right, 26, 13);

        _bl = CreateButton(DockPosition.BottomLeft, 0, 26);
        _bc = CreateButton(DockPosition.Bottom, 13, 26);
        _br = CreateButton(DockPosition.BottomRight, 26, 26);

        Controls.AddRange(new[]
        {
            _tl, _tc, _tr,
            _ml,      _mr,
            _bl, _bc, _br
        });

        OnSelectedDockChanged();
    }

    private Button CreateButton(DockPosition value, int x, int y)
    {
        var btn = new Button
        {
            Width = 12,
            Height = 12,
            Left = x,
            Top = y,
            BackColor = Color.LightGray,
            FlatStyle = FlatStyle.Flat,
            Tag = value
        };

        btn.Click += OnButtonClick;
        _buttons[value] = btn;
        return btn;
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        if (sender is not Button btn)
            return;

        if (btn.Tag is DockPosition pos)
        {
            SelectedDock = pos;
        }
    }

    private void UpdateButtonStates()
    {
        foreach (Control c in Controls)
        {
            if (c is Button btn && btn.Tag is DockPosition pos)
            {
                btn.BackColor = (pos == _selectedDock)
                    ? Color.SteelBlue
                    : SystemColors.Control;
            }
        }
    }

    private void UpdateVisualState()
    {
        foreach (var kvp in _buttons)
        {
            if (kvp.Key == _selectedDock)
            {
                kvp.Value.BackColor = Color.DeepSkyBlue;
                kvp.Value.FlatAppearance.BorderColor = Color.RoyalBlue;
            }
            else
            {
                kvp.Value.BackColor = Color.LightGray;
                kvp.Value.FlatAppearance.BorderColor = Color.Gray;
            }
        }
    }

    private void OnSelectedDockChanged()
    {
        UpdateButtonStates();
        UpdateVisualState();
        SelectedDockChanged?.Invoke(this, EventArgs.Empty);
    }
}
