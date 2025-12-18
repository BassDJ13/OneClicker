namespace PluginContracts;

public class MenuItem //todo: Create interface IMenuItem
{
    public string Description { get; }
    public Image? Image { get; }
    public EventHandler? OnClick { get; }

    public MenuItem(string description, Image? image, EventHandler? onClick) 
    {
        Description = description;
        Image = image;
        OnClick = onClick;
    }

    public MenuItem(string description, Image? image, Action onClick)
        : this(
            description,
            image,
            new EventHandler((_, _) => onClick())
          )
    { }
}
