using System;

namespace SiteHealthMonitor.Utilities
{
    public static class UriExtensions
    {
        public static string GetNormalizedHost(this Uri uri)
        {
            return uri.Host.Replace(":", "_").ToLowerInvariant();
        }
        
        public static bool IsValidRootUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) == false) return false;
        
            var isValidScheme = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            var isRoot = string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath == "/";
            
            return isValidScheme && isRoot;
        }

        public static string NormalizeUrl(string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == false && url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) == false)
            {
                url = "https://" + url;
            }

            Uri.TryCreate(url, UriKind.Absolute, out var uri);

            return $"{uri?.Scheme}://{uri?.Host}";
        }
    }
}