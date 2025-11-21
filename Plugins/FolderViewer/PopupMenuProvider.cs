namespace FolderViewer;

internal class PopupMenuProvider
{
    private static ContextMenuStrip? _menu;

    internal static ContextMenuStrip Menu =>
        _menu ??= new ContextMenuStrip();
}
