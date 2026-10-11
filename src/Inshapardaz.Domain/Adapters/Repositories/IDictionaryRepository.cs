using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Adapters.Repositories;

// Meaning dictionaries and their articles. Headword matching uses the normalised headword
// (WordNormaliser), which the repository stores alongside each article.
public interface IDictionaryRepository
{
    Task<Page<DictionaryModel>> GetDictionaries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<DictionaryModel> GetDictionaryById(long dictionaryId, CancellationToken cancellationToken);
    Task<DictionaryModel> AddDictionary(DictionaryModel dictionary, CancellationToken cancellationToken);
    Task<DictionaryModel> UpdateDictionary(DictionaryModel dictionary, CancellationToken cancellationToken);
    Task DeleteDictionary(long dictionaryId, CancellationToken cancellationToken);

    // Recomputes the dictionary's EntryCount from its articles.
    Task UpdateEntryCount(long dictionaryId, CancellationToken cancellationToken);

    Task AddArticles(long dictionaryId, IEnumerable<DictionaryArticleModel> articles, CancellationToken cancellationToken);
    Task<Page<DictionaryArticleModel>> GetArticles(long dictionaryId, string normalisedQuery, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<DictionaryArticleModel>> GetAllArticles(long dictionaryId, CancellationToken cancellationToken);
    Task<DictionaryArticleModel> GetArticleById(long dictionaryId, long articleId, CancellationToken cancellationToken);
    Task<DictionaryArticleModel> AddArticle(DictionaryArticleModel article, CancellationToken cancellationToken);
    Task<DictionaryArticleModel> UpdateArticle(DictionaryArticleModel article, CancellationToken cancellationToken);
    Task DeleteArticles(long dictionaryId, IEnumerable<long> articleIds, CancellationToken cancellationToken);

    // Articles whose headword matches exactly, across the given dictionaries (all when null).
    Task<IEnumerable<DictionaryArticleModel>> LookupArticles(string normalisedHeadword, IEnumerable<long> dictionaryIds, CancellationToken cancellationToken);

    // Headwords starting with the prefix, across the given dictionaries (all when null).
    Task<IEnumerable<string>> SuggestHeadwords(string normalisedPrefix, IEnumerable<long> dictionaryIds, int limit, CancellationToken cancellationToken);
}
