namespace Inshapardaz.Domain.Helpers;

public static class UrlAllowlistHelper
{
    // Rejects anything that isn't an https URL on the given host (or a subdomain of it),
    // so a caller of the Rekhta/Chughtai downloader tools can't point the server at
    // arbitrary internal infrastructure or cloud metadata endpoints (SSRF).
    public static bool IsAllowedHost(string url, string allowedHost)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        return uri.Host.Equals(allowedHost, StringComparison.OrdinalIgnoreCase)
            || uri.Host.EndsWith("." + allowedHost, StringComparison.OrdinalIgnoreCase);
    }
}
