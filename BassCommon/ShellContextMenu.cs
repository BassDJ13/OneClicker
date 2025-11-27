using System.Runtime.InteropServices;
using System.Text;

namespace BassCommon;

public class ShellContextMenu
{
    public void Show(string filePath, IntPtr hwndOwner, int x, int y)
    {
        IntPtr pidl = IntPtr.Zero;
        IntPtr parentFolderPtr = IntPtr.Zero;
        IntPtr contextMenuPtr = IntPtr.Zero;
        IntPtr hMenu = IntPtr.Zero;
        IntPtr pidlChild = IntPtr.Zero;

        try
        {
            // Get PIDL for file
            if (SHParseDisplayName(filePath, IntPtr.Zero, out pidl, 0, out _) != 0 || pidl == IntPtr.Zero)
            {
                return;
            }

            // Bind to parent folder
            Guid iidIShellFolder = typeof(IShellFolder).GUID;
            if (SHBindToParent(pidl, ref iidIShellFolder, out parentFolderPtr, out pidlChild) != 0 || parentFolderPtr == IntPtr.Zero)
            {
                return;
            }

            // Get IContextMenu for the item
            Guid iidIContextMenu = typeof(IContextMenu).GUID;
            IntPtr[] apidl = { pidlChild };
            if (((IShellFolder)Marshal.GetObjectForIUnknown(parentFolderPtr)).GetUIObjectOf(
                hwndOwner, 1, apidl, ref iidIContextMenu, IntPtr.Zero, out contextMenuPtr) != 0 || contextMenuPtr == IntPtr.Zero)
            {
                return;
            }

            var contextMenu = (IContextMenu)Marshal.GetObjectForIUnknown(contextMenuPtr);

            // Create popup menu
            hMenu = CreatePopupMenu();
            if (hMenu == IntPtr.Zero)
            {
                return;
            }

            contextMenu.QueryContextMenu(hMenu, 0, 1, 0x7FFF, 0);

            uint selected = TrackPopupMenuEx(hMenu, TPM_RETURNCMD, x, y, hwndOwner, IntPtr.Zero);
            if (selected > 0)
            {
                var invoke = new CMINVOKECOMMANDINFOEX
                {
                    cbSize = (uint)Marshal.SizeOf<CMINVOKECOMMANDINFOEX>(),
                    fMask = 0,
                    hwnd = hwndOwner,
                    lpVerb = (IntPtr)(selected - 1),
                    nShow = SW_SHOWNORMAL
                };
                contextMenu.InvokeCommand(ref invoke);
            }
        }
        finally
        {
            if (hMenu != IntPtr.Zero)
            {
                DestroyMenu(hMenu);
            }

            if (contextMenuPtr != IntPtr.Zero)
            {
                Marshal.Release(contextMenuPtr);
            }

            if (parentFolderPtr != IntPtr.Zero)
            {
                Marshal.Release(parentFolderPtr);
            }

            if (pidl != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pidl);
            }
        }
    }

    // --- P/Invoke and COM definitions ---

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
    private static extern int SHParseDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] string name,
        IntPtr pbc,
        out IntPtr ppidl,
        uint sfgaoIn,
        out uint psfgaoOut);

    [DllImport("shell32.dll")]
    private static extern int SHBindToParent(
        IntPtr pidl,
        ref Guid riid,
        out IntPtr ppv,
        out IntPtr ppidlLast);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyMenu(IntPtr hMenu);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint TrackPopupMenuEx(
        IntPtr hMenu,
        uint uFlags,
        int x,
        int y,
        IntPtr hwnd,
        IntPtr lptpm);

    private const uint TPM_RETURNCMD = 0x0100;
    private const int SW_SHOWNORMAL = 1;

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    private interface IShellFolder
    {
        void ParseDisplayName(IntPtr hwnd, IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
            ref uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);

        int EnumObjects(IntPtr hwnd, int grfFlags, out IntPtr ppenumIDList);

        int BindToObject(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv);

        int BindToStorage(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv);

        int CompareIDs(int lParam, IntPtr pidl1, IntPtr pidl2);

        int CreateViewObject(IntPtr hwndOwner, ref Guid riid, out IntPtr ppv);

        int GetAttributesOf(int cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] apidl, ref uint rgfInOut);

        int GetUIObjectOf(IntPtr hwndOwner, int cidl,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] apidl,
            ref Guid riid, IntPtr rgfReserved, out IntPtr ppv);

        int GetDisplayNameOf(IntPtr pidl, uint uFlags, out STRRET pName);

        int SetNameOf(IntPtr hwnd, IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint uFlags, out IntPtr ppidlOut);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    private interface IContextMenu
    {
        int QueryContextMenu(IntPtr hMenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);

        void InvokeCommand(ref CMINVOKECOMMANDINFOEX pici);

        void GetCommandString(UIntPtr idCmd, uint uType, uint pReserved, StringBuilder pszName, uint cchMax);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CMINVOKECOMMANDINFOEX
    {
        public uint cbSize;
        public uint fMask;
        public IntPtr hwnd;
        public IntPtr lpVerb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDirectory;
        public int nShow;
        public uint dwHotKey;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpTitle;
        public IntPtr lpVerbW;
        public string lpParametersW;
        public string lpDirectoryW;
        public string lpTitleW;
        public POINT ptInvoke;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STRRET
    {
        public uint uType;
        public IntPtr pOleStr;
    }
}