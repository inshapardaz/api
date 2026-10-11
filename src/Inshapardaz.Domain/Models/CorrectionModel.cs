namespace Inshapardaz.Domain.Models;

public class CorrectionModel
{
    public long Id { get; set; }
    public string Language { get; set; }
    public string Profile { get; set; }
    public string IncorrectText { get; set; }
    public string CorrectText { get; set; }
    public bool CompleteWord { get; set; }

    // How many times editors have applied this correction; the editors' lists put the most used first.
    public long Usage { get; set; }
}

// The correction lists that exist per language.
public static class CorrectionProfiles
{
    public const string Autocorrect = "autocorrect";
    public const string Punctuation = "punctuation";

    public static readonly IReadOnlyList<string> All = [Autocorrect, Punctuation];

    public static bool IsValid(string profile) => All.Contains(profile);
}

public enum CorrectionSortByType
{
    IncorrectText,
    Usage,
}
