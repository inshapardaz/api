using System.ComponentModel.DataAnnotations;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Api.Views.Tools;

public class DictionaryView : ViewWithLinks
{
    public long Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Name { get; set; }

    [StringLength(2000)]
    public string Description { get; set; }

    public DictionaryFormat Format { get; set; }

    [StringLength(100)]
    public string SourceLanguage { get; set; }

    [StringLength(100)]
    public string TargetLanguage { get; set; }

    public int EntryCount { get; set; }
    public int? OwnerId { get; set; }
    public DateTime DateCreated { get; set; }
}

public class DictionaryArticleView : ViewWithLinks
{
    public long Id { get; set; }
    public long DictionaryId { get; set; }

    // Set on lookups across dictionaries.
    public string DictionaryName { get; set; }

    [Required]
    [StringLength(500)]
    public string Headword { get; set; }

    [StringLength(500)]
    public string Pronunciation { get; set; }

    public IEnumerable<WordSenseView> Senses { get; set; }
    // Synonyms, antonyms and other related words ("links" is taken by the HATEOAS links).
    public IEnumerable<WordLinkView> WordLinks { get; set; }
    public string RawDefinition { get; set; }
    public IEnumerable<string> Alternates { get; set; }
}

public class WordSenseView
{
    public string PartOfSpeech { get; set; }
    public string Grammar { get; set; }
    public IEnumerable<MeaningView> Meanings { get; set; }
}

public class MeaningView
{
    public string Definition { get; set; }
    public IEnumerable<string> Examples { get; set; }
    public string Label { get; set; }
}

public class WordLinkView
{
    public WordLinkType LinkType { get; set; }
    public string TargetWord { get; set; }
    public string Note { get; set; }
}

public class MergeArticlesView
{
    [Required]
    public IEnumerable<long> ArticleIds { get; set; }

    public string Headword { get; set; }
}

public class MergeCandidateGroupView
{
    public string BaseHeadword { get; set; }
    public IEnumerable<DictionaryArticleView> Articles { get; set; }
}

public class HeadwordSuggestionsView
{
    public IEnumerable<string> Suggestions { get; set; }
}
