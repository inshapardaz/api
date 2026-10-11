namespace Inshapardaz.Domain.Models.Transliteration;

public enum TransliterationScript
{
    Roman,
    UrduArabic,
    HindiDevanagari,
}

// The languages with a transliteration dictionary, and the script their entries are written in.
// Roman input is looked up in the dictionary of the language whose script it is converted to.
public static class TransliterationLanguages
{
    private static readonly Dictionary<string, TransliterationScript> Scripts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ur"] = TransliterationScript.UrduArabic,
        ["hi"] = TransliterationScript.HindiDevanagari,
    };

    public static IEnumerable<string> All => Scripts.Keys;

    public static bool TryGetScript(string language, out TransliterationScript script) => Scripts.TryGetValue(language ?? string.Empty, out script);
}

// A word whose romanised spelling should convert to a known native spelling instead of the
// mechanical rule-based result (e.g. "khuda" -> "خدا").
public class TransliterationEntryModel
{
    public long Id { get; set; }
    public string Language { get; set; }

    // Lower-cased; may be a phrase of up to three words.
    public string Roman { get; set; }
    public string Native { get; set; }
    public string Meaning { get; set; }
    public string Category { get; set; }
}

public class TransliterationResultModel
{
    public string Result { get; set; }
    public TransliterationScript From { get; set; }
    public TransliterationScript To { get; set; }

    // Word-by-word breakdown, only when details were asked for.
    public List<TransliteratedWordModel> Words { get; set; }
}

public class TransliteratedWordModel
{
    public string Input { get; set; }
    public string Output { get; set; }

    // "dictionary", "rules" or "punctuation".
    public string Source { get; set; }
    public long? EntryId { get; set; }
}

public class TransliterationImportResultModel
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
}
