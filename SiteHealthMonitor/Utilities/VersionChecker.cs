namespace SiteHealthMonitor.Utilities;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class VersionChecker
{
    private readonly string _apiUrl = "https://api.github.com/repos/{user}/{repo}/releases/latest";
    private readonly HttpClient _httpClient;
    private readonly string _currentVersion;

    public VersionChecker(string user, string repo, string currentVersion)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "SiteHealthMonitor");
        _apiUrl = _apiUrl.Replace("{user}", user).Replace("{repo}", repo);
        _currentVersion = currentVersion;
    }

    public async Task<bool> IsUpdateAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            var release = JsonConvert.DeserializeObject<GitHubRelease>(response);
            return CompareVersions(release!.TagName, _currentVersion) > 0;
        }
        catch
        {
            return false;
        }
    }

    private int CompareVersions(string gitVersion, string currentVersion)
    {
        var cleanGit = gitVersion.Trim().TrimStart('v');
        var cleanCurrent = currentVersion.Trim().TrimStart('v');
        
        var versionGit = new Version(cleanGit);
        var versionCurrent = new Version(cleanCurrent);
        
        return versionGit.CompareTo(versionCurrent);
    }

    private class GitHubRelease(string tagName)
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; } = tagName;
    }
}