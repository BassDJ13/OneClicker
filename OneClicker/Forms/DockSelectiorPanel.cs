using PluginContracts;

namespace OneClicker.Forms;

public class DockSelectorPanel : Panel
{
    private readonly Dictionary<DockPosition, Button> _buttons = new();
    public DockPosition Selected { get; private set; } = DockPosition.BottomRight;

    public event Action<DockPosition>? SelectionChanged;

    public DockSelectorPanel()
    {
        Size = new Size(40, 40);

        CreateButton(DockPosition.TopLeft, 0, 0);
        CreateButton(DockPosition.Top, 13, 0);
        CreateButton(DockPosition.TopRight, 26, 0);

        CreateButton(DockPosition.Left, 0, 13);

        CreateButton(DockPosition.Right, 26, 13);

        CreateButton(DockPosition.BottomLeft, 0, 26);
        CreateButton(DockPosition.Bottom, 13, 26);
        CreateButton(DockPosition.BottomRight, 26, 26);

        UpdateVisualState();
    }

    private void CreateButton(DockPosition pos, int x, int y)
    {
        var btn = new Button
        {
            Width = 12,
            Height = 12,
            Left = x,
            Top = y,
            BackColor = Color.LightGray,
            FlatStyle = FlatStyle.Flat,
            Tag = pos
        };

        btn.FlatAppearance.BorderSize = 1;
        btn.Click += (s, e) =>
        {
            Selected = pos;
            UpdateVisualState();
            SelectionChanged?.Invoke(Selected);
        };

        Controls.Add(btn);
        _buttons[pos] = btn;
    }

    private void UpdateVisualState()
    {
        foreach (var kvp in _buttons)
        {
            if (kvp.Key == Selected)
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

    public void SetPosition(DockPosition pos)
    {
        Selected = pos;
        UpdateVisualState();
    }
}
