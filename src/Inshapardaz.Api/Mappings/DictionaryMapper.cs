using System.Text.Json;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Api.Mappings;

public static class DictionaryMapper
{
    public static DictionaryView Map(this DictionaryModel source)
        => new DictionaryView
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Format = source.Format,
            SourceLanguage = source.SourceLanguage,
            TargetLanguage = source.TargetLanguage,
            EntryCount = source.EntryCount,
            OwnerId = source.OwnerId,
            DateCreated = source.DateCreated
        };

    public static DictionaryModel Map(this DictionaryView source)
        => new DictionaryModel
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            SourceLanguage = source.SourceLanguage,
            TargetLanguage = source.TargetLanguage
        };

    public static DictionaryArticleView Map(this DictionaryArticleModel source)
        => new DictionaryArticleView
        {
            Id = source.Id,
            DictionaryId = source.DictionaryId,
            DictionaryName = source.DictionaryName,
            Headword = source.Headword,
            Pronunciation = source.Pronunciation,
            Senses = source.Senses?.Select(s => new WordSenseView
            {
                PartOfSpeech = s.PartOfSpeech,
                Grammar = s.Grammar,
                Meanings = s.Meanings?.Select(m => new MeaningView { Definition = m.Definition, Examples = m.Examples, Label = m.Label }).ToList()
            }).ToList(),
            WordLinks = source.Links?.Select(l => new WordLinkView { LinkType = l.LinkType, TargetWord = l.TargetWord, Note = l.Note }).ToList(),
            RawDefinition = source.RawDefinition,
            Alternates = ParseAlternates(source.Alternates)
        };

    public static DictionaryArticleModel Map(this DictionaryArticleView source)
        => new DictionaryArticleModel
        {
            Id = source.Id,
            DictionaryId = source.DictionaryId,
            Headword = source.Headword?.Trim(),
            Pronunciation = source.Pronunciation,
            Senses = source.Senses?.Select(s => new WordSense
            {
                PartOfSpeech = s.PartOfSpeech,
                Grammar = s.Grammar,
                Meanings = s.Meanings?.Select(m => new Meaning { Definition = m.Definition, Examples = m.Examples?.ToList() ?? [], Label = m.Label }).ToList() ?? []
            }).ToList() ?? [],
            Links = source.WordLinks?.Select(l => new WordLink { LinkType = l.LinkType, TargetWord = l.TargetWord, Note = l.Note }).ToList() ?? [],
            RawDefinition = source.RawDefinition,
            Alternates = source.Alternates?.Any() == true ? JsonSerializer.Serialize(source.Alternates) : null
        };

    private static IEnumerable<string> ParseAlternates(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
