using System.Text.Json;

namespace BassCommon.Classes;

public static class GitHubUpdateChecker
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

    public static Version GetVersion()
    {
        var versionString = Application.ProductVersion;
        if (string.IsNullOrWhiteSpace(versionString))
        {
            return new Version(0, 0, 0, 0);
        }

        // remove "+gitsha"
        int pos = versionString.IndexOf('+');
        if (pos >= 0)
        {
            versionString = versionString[..pos];
        }

        if (Version.TryParse(versionString, out var version))
        {
            return version;
        }

        return new Version(0, 0, 0);
    }
}
