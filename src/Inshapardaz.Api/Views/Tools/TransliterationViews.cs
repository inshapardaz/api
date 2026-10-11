using System.ComponentModel.DataAnnotations;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Api.Views.Tools;

public class TransliterateRequestView
{
    [Required]
    public string Text { get; set; }

    public TransliterationScript From { get; set; }
    public TransliterationScript To { get; set; }

    // Also return a word-by-word breakdown.
    public bool Details { get; set; }
}

public class TransliterationResultView
{
    public string Result { get; set; }
    public TransliterationScript From { get; set; }
    public TransliterationScript To { get; set; }
    public IEnumerable<TransliteratedWordView> Words { get; set; }
}

public class TransliteratedWordView
{
    public string Input { get; set; }
    public string Output { get; set; }
    public string Source { get; set; }
    public long? EntryId { get; set; }
}

public class TransliterationEntryView : ViewWithLinks
{
    public long Id { get; set; }
    public string Language { get; set; }

    [Required]
    [StringLength(200)]
    public string Roman { get; set; }

    [Required]
    [StringLength(200)]
    public string Native { get; set; }

    [StringLength(500)]
    public string Meaning { get; set; }

    [StringLength(100)]
    public string Category { get; set; }
}

public class TransliterationImportResultView
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
}
