using BassCommon;

namespace FolderViewer;

internal class PopupMenuProvider
{
    private static ContextMenuStrip? _menu;
    private static readonly ShellContextMenu shellContextMenu = new();

    internal static ContextMenuStrip Menu =>
        _menu ??= CreateMenu();

    private static ContextMenuStrip CreateMenu()
    {
        var menu = new ContextMenuStrip();
        menu.MouseUp += PopupMenu_MouseUp;
        return menu;
    }

    private static void PopupMenu_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        var menu = sender as ContextMenuStrip;
        if (menu == null)
        {
            return;
        }

        var item = menu.GetItemAt(e.Location);
        if (item?.Tag is not string path)
        {
            return;
        }

        var screen = menu.PointToScreen(e.Location);

        shellContextMenu.Show(path, menu.Handle, screen.X, screen.Y);
    }
}
