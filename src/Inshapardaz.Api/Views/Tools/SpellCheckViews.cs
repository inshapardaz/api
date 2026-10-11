using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Tools;

public class SpellCheckRequestView
{
    [Required]
    public IEnumerable<string> Words { get; set; }
}

public class SpellCheckResultView
{
    public string Word { get; set; }
    public bool Found { get; set; }
    public IEnumerable<SpellingSuggestionView> Suggestions { get; set; }
}

public class SpellingSuggestionView
{
    public string Word { get; set; }
    public int Priority { get; set; }
}

public class SpellingReplacementView
{
    [Required]
    public string Word { get; set; }

    [Required]
    public string Replacement { get; set; }
}

public class WordImportRequestView
{
    [Required]
    public IEnumerable<string> Words { get; set; }
}

public class WordImportResultView
{
    public int Added { get; set; }
    public int Skipped { get; set; }
}
