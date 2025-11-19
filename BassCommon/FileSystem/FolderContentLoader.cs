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

    private static Icon GetFileIcon(string path)
    {
        SHFILEINFO shinfo = new SHFILEINFO();
        SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
            SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
        return shinfo.hIcon != nint.Zero ? Icon.FromHandle(shinfo.hIcon) : SystemIcons.Application;
    }

    public static IEnumerable<ToolStripMenuItem> GetItems(string folderPath)
    {
        var items = new List<ToolStripMenuItem>();

        foreach (var item in Directory.GetFileSystemEntries(folderPath))
        {
            if (Path.GetFileName(item) == "desktop.ini")
            {
                continue;
            }
            var name = Path.GetFileNameWithoutExtension(item);
            try
            {
                using (var icon = GetFileIcon(item))
                {
                    items.Add(new ToolStripMenuItem(name)
                    {
                        Tag = item,
                        Image = icon.ToBitmap()
                    });
                }
            }
            catch
            {
                items.Add(new ToolStripMenuItem(name) { Tag = item });
            }
        }

        return items;
    }
}
