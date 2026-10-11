using System.Text.Json;
using System.Text.Json.Serialization;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Helpers.Dictionaries;

// A dictionary article as stored: senses and links as JSON text, plus the normalised headword used
// for lookups. Shared by the MySQL and SQL Server repositories so both store the same shape.
public class DictionaryArticleRow
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    public long Id { get; set; }
    public long DictionaryId { get; set; }
    public string DictionaryName { get; set; }
    public string Headword { get; set; }
    public string HeadwordNormalised { get; set; }
    public string Pronunciation { get; set; }
    public string Senses { get; set; }
    public string Links { get; set; }
    public string RawDefinition { get; set; }
    public string Alternates { get; set; }

    public static DictionaryArticleRow From(DictionaryArticleModel article, long dictionaryId) => new()
    {
        Id = article.Id,
        DictionaryId = dictionaryId,
        Headword = article.Headword?.Trim(),
        HeadwordNormalised = WordNormaliser.Normalise(article.Headword),
        Pronunciation = article.Pronunciation,
        Senses = article.Senses is { Count: > 0 } ? JsonSerializer.Serialize(article.Senses, JsonOptions) : null,
        Links = article.Links is { Count: > 0 } ? JsonSerializer.Serialize(article.Links, JsonOptions) : null,
        RawDefinition = article.RawDefinition,
        Alternates = article.Alternates,
    };

    public DictionaryArticleModel ToModel() => new()
    {
        Id = Id,
        DictionaryId = DictionaryId,
        DictionaryName = DictionaryName,
        Headword = Headword,
        Pronunciation = Pronunciation,
        Senses = Deserialize<List<WordSense>>(Senses) ?? [],
        Links = Deserialize<List<WordLink>>(Links) ?? [],
        RawDefinition = RawDefinition,
        Alternates = Alternates,
    };

    private static T Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
