namespace OneClicker.Classes;

internal static class GlobalHotkeyRegistry
{
    private static readonly HashSet<Keys> _reserved = new()
    {
        Keys.Alt | Keys.F4,
        Keys.Control | Keys.Escape
    };

    internal static bool IsTaken(Keys key) => _reserved.Contains(key);

    internal static void Register(Keys key)
    {
        _reserved.Add(key);
    }
}
