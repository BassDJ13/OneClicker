using System.Text.Json;

namespace OneClicker.Classes;

internal static class GitHubUpdateChecker
{
    public static async Task<string?> GetLatestVersionTagAsync(string owner, string name)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("OneClicker-App");

        var url = $"https://api.github.com/repos/{owner}/{name}/releases/latest";
        try
        {
            var json = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("tag_name").GetString();
        }
        catch
        {
            return null;
        }
    }

    public static async Task<bool> IsLatestVersionAsync(string owner, string name, Version currentVersion)
    {
        var tag = await GetLatestVersionTagAsync(owner, name);
        if (string.IsNullOrEmpty(tag))
        {
            return true;
        }

        if (Version.TryParse(tag.TrimStart('v', 'V'), out var latest))
        {
            return currentVersion >= latest;
        }

        return true;
    }
}
