using PluginContracts;

namespace OneClicker.WindowBehavior
{
    public interface IWindowLocationHelper
    {
        void EnsureVisible(Form form);
        Point GetDockedPosition(Rectangle workingArea, Size windowSize, DockPosition position, int offsetX, int offsetY);
        void KeepInWorkArea(Form form);
    }
}