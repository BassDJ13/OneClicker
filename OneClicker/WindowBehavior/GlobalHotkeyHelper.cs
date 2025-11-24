using System.Runtime.InteropServices;

namespace OneClicker.WindowBehavior;

public sealed class GlobalHotkeyHelper : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private readonly IntPtr _handle;
    private readonly int _hotkeyId;
    private readonly Action _onHotkeyPressed;
    private bool _isRegistered;

    public GlobalHotkeyHelper(IntPtr handle, Action onHotkeyPressed, Keys keyData)
    {
        _handle = handle;
        _onHotkeyPressed = onHotkeyPressed ?? throw new ArgumentNullException(nameof(onHotkeyPressed));
        _hotkeyId = GetHashCode();
        var key = keyData & Keys.KeyCode;
        var modifiers = keyData & Keys.Modifiers;
        _isRegistered = RegisterHotKey(handle, _hotkeyId, GetModifiers(modifiers), (uint)key);

        if (!_isRegistered)
        {
            //MessageBox.Show("Unable to register global hotkey.", "Warning",
            //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private uint GetModifiers(Keys keyData)
    {
        uint mods = 0;
        Keys modifiers = keyData & Keys.Modifiers;

        if (modifiers.HasFlag(Keys.Alt))
            mods |= MOD_ALT;
        if (modifiers.HasFlag(Keys.Control))
            mods |= MOD_CONTROL;
        if (modifiers.HasFlag(Keys.Shift))
            mods |= MOD_SHIFT;
        if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin))
            mods |= MOD_WIN;

        return mods;
    }

    public bool ProcessHotkeyMessage(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == _hotkeyId)
        {
            _onHotkeyPressed?.Invoke();
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        if (_isRegistered)
        {
            UnregisterHotKey(_handle, _hotkeyId);
            _isRegistered = false;
        }
    }
}
