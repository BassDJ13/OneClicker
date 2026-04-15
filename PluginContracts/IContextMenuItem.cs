namespace PluginContracts;

public interface IContextMenuItem
{
    string Description { get; set; }
    Image? Image { get; set; }
    EventHandler? OnClick { get; }
}