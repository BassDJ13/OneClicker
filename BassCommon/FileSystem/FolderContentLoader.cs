using System.Runtime.InteropServices;

namespace BassCommon.FileSystem;

public static class FolderContentLoader
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern nint SHGetFileInfo(
        string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
        uint cbFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public nint hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_SMALLICON = 0x1;
    private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
    private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
    private const uint SHGFI_LARGEICON = 0x0;

    private static Icon GetFolderIcon()
    {
        SHFILEINFO shinfo = new SHFILEINFO();

        SHGetFileInfo(
            string.Empty,
            FILE_ATTRIBUTE_DIRECTORY,
            ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES
        );

        return shinfo.hIcon != nint.Zero
            ? Icon.FromHandle(shinfo.hIcon)
            : SystemIcons.WinLogo;
    }

    private static Icon GetFileIcon(string path)
    {
        SHFILEINFO shinfo = new SHFILEINFO();
        SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
            SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
        return shinfo.hIcon != nint.Zero ? Icon.FromHandle(shinfo.hIcon) : SystemIcons.Application;
    }

    public static IEnumerable<ToolStripMenuItem> GetItems(
        string folderPath,
        EventHandler fileClickHandler,
        MouseEventHandler rightClickHandler)
    {
        var entries = Directory.GetFileSystemEntries(folderPath)
            .Where(p => Path.GetFileName(p) != "desktop.ini")
            .OrderByDescending(Directory.Exists)
            .ThenBy(Path.GetFileName);

        foreach (var path in entries)
        {
            if (Directory.Exists(path) && !IsGodModeFolder(path))
            {
                yield return CreateFolderMenuItem(
                    path,
                    fileClickHandler,
                    rightClickHandler);
            }
            else
            {
                yield return CreateFileMenuItem(
                    path,
                    fileClickHandler,
                    rightClickHandler);
            }
        }
    }

    private static ToolStripMenuItem CreateFolderMenuItem(
        string folderPath,
        EventHandler fileClickHandler,
        MouseEventHandler rightClickHandler)
    {
        var folderItem = new ToolStripMenuItem(Path.GetFileName(folderPath))
        {
            Tag = folderPath,
            Image = GetFolderIcon().ToBitmap()
        };

        folderItem.DropDownItems.Add("Loading...");

        folderItem.DropDownOpening += (s, e) =>
        {
            var item = (ToolStripMenuItem)s!;

            if (item.DropDownItems.Count == 1 &&
                item.DropDownItems[0].Text == "Loading...")
            {
                item.DropDownItems.Clear();

                try
                {
                    foreach (var child in GetItems(
                        folderPath,
                        fileClickHandler,
                        rightClickHandler))
                    {
                        item.DropDownItems.Add(child);
                    }
                }
                catch
                {
                    item.DropDownItems.Add(
                        new ToolStripMenuItem("(Access denied)") { Enabled = false });
                }
            }
        };

        return folderItem;
    }

    private static ToolStripMenuItem CreateFileMenuItem(
        string filePath,
        EventHandler fileClickHandler,
        MouseEventHandler rightClickHandler)
    {
        var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(filePath))
        {
            Tag = filePath
        };

        item.Click += fileClickHandler;
        item.MouseDown += rightClickHandler;

        try
        {
            using var icon = GetFileIcon(filePath);
            item.Image = icon.ToBitmap();
        }
        catch { }

        return item;
    }

    private static bool IsGodModeFolder(string path)
    {
        return path.EndsWith(
            ".{ED7BA470-8E54-465E-825C-99712043E01C}",
            StringComparison.OrdinalIgnoreCase);
    }

    //private static bool IsShellFolder(string path)
    //{
    //    return Path.GetFileName(path).Contains(".{", StringComparison.Ordinal);
    //}
}
