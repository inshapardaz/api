namespace Inshapardaz.Domain.Models.Dictionaries;

// The format a dictionary was imported from; Manual for dictionaries built through the API.
public enum DictionaryFormat
{
    StarDict,
    Dsl,
    Unknown,
    Manual,
}

public enum DictionaryExportFormat
{
    StarDict,
    Dsl,
    Json,
    Kobo,
    Kindle,
}

// A meaning dictionary. Every dictionary is readable by everyone; only its owner (or an Admin) may
// change it.
public class DictionaryModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DictionaryFormat Format { get; set; }
    public string SourceLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public int EntryCount { get; set; }
    public int? OwnerId { get; set; }
    public string OriginalFileName { get; set; }
    public DateTime DateCreated { get; set; }
}

public class DictionaryArticleModel
{
    public long Id { get; set; }
    public long DictionaryId { get; set; }

    // Filled in by lookups across dictionaries, so results can say where they came from.
    public string DictionaryName { get; set; }

    public string Headword { get; set; }
    public string Pronunciation { get; set; }

    // Senses grouped by part of speech; a word can be both a noun and a verb.
    public List<WordSense> Senses { get; set; } = [];

    // Synonyms, antonyms, roots, derived forms, cross-references.
    public List<WordLink> Links { get; set; } = [];

    // The definition as it appeared in the source (HTML or text), for display when the structured
    // senses could not be extracted.
    public string RawDefinition { get; set; }

    // Alternate headwords (spellings) as a JSON array of strings.
    public string Alternates { get; set; }
}

public class WordSense
{
    public string PartOfSpeech { get; set; }
    public string Grammar { get; set; }
    public List<Meaning> Meanings { get; set; } = [];
}

public class Meaning
{
    public string Definition { get; set; }
    public List<string> Examples { get; set; } = [];
    public string Label { get; set; }
}

public class WordLink
{
    public WordLinkType LinkType { get; set; }
    public string TargetWord { get; set; }
    public string Note { get; set; }
}

public enum WordLinkType
{
    Synonym,
    Antonym,
    Root,
    DerivedForm,
    Related,
    Narrower,
    Broader,
    SeeAlso,
}

public class ParsedDictionaryModel
{
    public DictionaryModel Info { get; set; }
    public IReadOnlyList<DictionaryArticleModel> Articles { get; set; }
}

public class ExportedFileModel
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
}

public class MergeCandidateGroupModel
{
    public string BaseHeadword { get; set; }
    public IEnumerable<DictionaryArticleModel> Articles { get; set; }
}
