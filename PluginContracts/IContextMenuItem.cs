namespace PluginContracts;

public interface IContextMenuItem
{
    string Description { get; }
    Image? Image { get; }
    EventHandler? OnClick { get; }
}