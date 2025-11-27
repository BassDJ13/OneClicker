using Vanara.PInvoke;
using Vanara.InteropServices;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Shell32;

namespace BassCommon;

public class ShellContextMenu
{
    public void Show(string filePath, IntPtr hwndOwner, int x, int y)
    {
        if (!SHParseDisplayName(filePath, IntPtr.Zero, out var pidl, 0, out _).Succeeded)
        {
            return;
        }


        if (SHBindToParent(pidl, typeof(IShellFolder).GUID, out var parentObj, out var pidlChild).Failed)
        {
            return;
        }

        using (var parent = new ComReleaser<IShellFolder>((IShellFolder)parentObj!))
        {
            var iidIContextMenu = typeof(IContextMenu).GUID;
            var apidl = new[] { pidlChild };
            if (parent.Item.GetUIObjectOf(hwndOwner, 1, apidl, ref iidIContextMenu, IntPtr.Zero, out var oContextMenu).Failed)
            {
                return;
            }

            using (var contextMenu = new ComReleaser<IContextMenu>((IContextMenu)oContextMenu!))
            {
                var hMenu = CreatePopupMenu();
                try
                {
                    contextMenu.Item.QueryContextMenu(hMenu, 0, 1, 0x7FFF, 0);
                    uint selected = TrackPopupMenuEx(hMenu, TrackPopupMenuFlags.TPM_RETURNCMD, x, y, hwndOwner);
                    if (selected > 0)
                    {
                        var invoke = new CMINVOKECOMMANDINFOEX
                        {
                            cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<CMINVOKECOMMANDINFOEX>(),
                            lpVerb = (IntPtr)(selected - 1),
                            nShow = ShowWindowCommand.SW_SHOWNORMAL
                        };
                        contextMenu.Item.InvokeCommand(in invoke);
                    }
                }
                finally
                {
                    DestroyMenu(hMenu);
                }
            }
        }
    }
}