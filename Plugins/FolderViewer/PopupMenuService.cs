namespace FolderViewer;

internal class PopupMenuService
{
    private static ContextMenuStrip? _popupMenu;
    internal static ContextMenuStrip PopupMenu =>
        _popupMenu ??= _popupMenu = new ContextMenuStrip();
}
