using System.Text.RegularExpressions;

namespace Inshapardaz.Domain.Helpers.Dictionaries;

public static partial class HeadwordGrouping
{
    // "bank 1", "bank[2]", "bank (3)" -> "bank": the form shared by numbered homographs.
    public static string BaseHeadword(string headword) =>
        string.IsNullOrWhiteSpace(headword) ? headword : NumberSuffix().Replace(headword.Trim(), string.Empty).Trim();

    [GeneratedRegex(@"\s*[\[\(]?\d+[\]\)]?\s*$")]
    private static partial Regex NumberSuffix();
}
