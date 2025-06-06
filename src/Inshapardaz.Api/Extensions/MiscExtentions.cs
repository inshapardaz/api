using System.Text;
using Inshapardaz.Api.Views;
using System.Text.RegularExpressions;

namespace Inshapardaz.Api.Extensions;

public static class MiscExtentions
{
    public static Uri ToUri(this string url)
    {
        return new Uri(url);
    }

    public static string Self(this IEnumerable<LinkView> links)
    {
        return links.SingleOrDefault(l => l.Rel == RelTypes.Self)?.Href;
    }

    public static string MaskEmail(this string source)
    {
        if (string.IsNullOrEmpty(source)) return source;
        string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
        return Regex.Replace(source, pattern, m => new string('*', m.Length));
    }
}
