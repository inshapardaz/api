namespace Inshapardaz.Domain.Models;

public class SpellCheckResultModel
{
    public string Word { get; set; }
    public bool Found { get; set; }
    public IEnumerable<SpellingSuggestionModel> Suggestions { get; set; } = [];
}

public class SpellingSuggestionModel
{
    public string Word { get; set; }

    // How many times writers have chosen this replacement; 0 for dictionary suggestions.
    public int Priority { get; set; }
}

public class SpellingReplacementModel
{
    public string Language { get; set; }
    public string SourceWord { get; set; }
    public string Replacement { get; set; }
    public int Count { get; set; }
}

public class WordImportResultModel
{
    public int Added { get; set; }
    public int Skipped { get; set; }
}
