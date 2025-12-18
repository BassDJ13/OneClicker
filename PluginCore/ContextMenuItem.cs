using PluginContracts;

namespace PluginCore;

public class ContextMenuItem : IContextMenuItem
{
    public string Description { get; }
    public Image? Image { get; }
    public EventHandler? OnClick { get; }

    public ContextMenuItem(string description, Image? image, EventHandler? onClick)
    {
        Description = description;
        Image = image;
        OnClick = onClick;
    }

    public ContextMenuItem(string description, Image? image, Action onClick)
        : this(
            description,
            image,
            new EventHandler((_, _) => onClick())
          )
    { }
}
