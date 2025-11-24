namespace OneClicker.Classes;

internal static class KeyParser
{
    internal static string ToSettingString(Keys keyData)
    {
        if (keyData == Keys.None)
        {
            return "";
        }

        var parts = GetParts(keyData);
        return string.Join(" + ", parts);
    }

    internal static IList<string> GetParts(Keys keyData)
    {
        if (keyData == Keys.None)
        {
            return [];
        }

        var parts = new List<string>();
        var key = keyData & Keys.KeyCode;
        var modifiers = keyData & Keys.Modifiers;

        if (modifiers.HasFlag(Keys.Control))
        {
            parts.Add("Ctrl");
        }

        if (modifiers.HasFlag(Keys.Alt))
        {
            parts.Add("Alt");
        }

        if (modifiers.HasFlag(Keys.Shift))
        {
            parts.Add("Shift");
        }

        if (key != Keys.None)
        {
            parts.Add(key.ToString());
        }

        return parts;
    }

    internal static Keys FromSettingString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Keys.None;
        }

        var result = Keys.None;

        var parts = text.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            switch (part.ToLower())
            {
                case "ctrl":
                case "control":
                    result |= Keys.Control;
                    break;

                case "shift":
                    result |= Keys.Shift;
                    break;

                case "alt":
                    result |= Keys.Alt;
                    break;

                case "win":
                case "windows":
                    result |= Keys.LWin; // Common choice
                    break;

                default:
                    if (Enum.TryParse<Keys>(part, true, out var key))
                    {
                        result |= key;
                    }
                    break;
            }
        }

        return result;
    }
}