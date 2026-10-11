using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.Dictionaries;

// All dictionary reads are public.

public class GetDictionariesQuery(string query, int pageNumber, int pageSize) : IQuery<Page<DictionaryModel>>
{
    public string Query { get; } = query;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}

public class GetDictionariesQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<GetDictionariesQuery, Page<DictionaryModel>>
{
    public override async Task<Page<DictionaryModel>> ExecuteAsync(GetDictionariesQuery query, CancellationToken cancellationToken = default)
        => await dictionaryRepository.GetDictionaries(query.Query, query.PageNumber, query.PageSize, cancellationToken);
}

public class GetDictionaryByIdQuery(long dictionaryId) : IQuery<DictionaryModel>
{
    public long DictionaryId { get; } = dictionaryId;
}

public class GetDictionaryByIdQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<GetDictionaryByIdQuery, DictionaryModel>
{
    public override async Task<DictionaryModel> ExecuteAsync(GetDictionaryByIdQuery query, CancellationToken cancellationToken = default)
        => await dictionaryRepository.GetDictionaryById(query.DictionaryId, cancellationToken);
}

// Articles of one dictionary in headword order, optionally only those whose headword contains Query.
public class GetDictionaryArticlesQuery(long dictionaryId, string query, int pageNumber, int pageSize) : IQuery<Page<DictionaryArticleModel>>
{
    public long DictionaryId { get; } = dictionaryId;
    public string Query { get; } = query;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}

public class GetDictionaryArticlesQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<GetDictionaryArticlesQuery, Page<DictionaryArticleModel>>
{
    public override async Task<Page<DictionaryArticleModel>> ExecuteAsync(GetDictionaryArticlesQuery query, CancellationToken cancellationToken = default)
        => await dictionaryRepository.GetArticles(query.DictionaryId, WordNormaliser.Normalise(query.Query), query.PageNumber, query.PageSize, cancellationToken);
}

public class GetDictionaryArticleByIdQuery(long dictionaryId, long articleId) : IQuery<DictionaryArticleModel>
{
    public long DictionaryId { get; } = dictionaryId;
    public long ArticleId { get; } = articleId;
}

public class GetDictionaryArticleByIdQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<GetDictionaryArticleByIdQuery, DictionaryArticleModel>
{
    public override async Task<DictionaryArticleModel> ExecuteAsync(GetDictionaryArticleByIdQuery query, CancellationToken cancellationToken = default)
        => await dictionaryRepository.GetArticleById(query.DictionaryId, query.ArticleId, cancellationToken);
}

// The meaning of a word: matching articles from the chosen dictionaries (all when none are given).
public class LookupWordQuery(string word, IEnumerable<long> dictionaryIds) : IQuery<IEnumerable<DictionaryArticleModel>>
{
    public string Word { get; } = word;
    public IEnumerable<long> DictionaryIds { get; } = dictionaryIds;
}

public class LookupWordQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<LookupWordQuery, IEnumerable<DictionaryArticleModel>>
{
    public override async Task<IEnumerable<DictionaryArticleModel>> ExecuteAsync(LookupWordQuery query, CancellationToken cancellationToken = default)
    {
        var headword = WordNormaliser.Normalise(query.Word);
        if (headword.Length == 0) throw new BadRequestException("A word to look up is required.");

        return await dictionaryRepository.LookupArticles(headword, query.DictionaryIds, cancellationToken);
    }
}

// Headwords that start with Prefix, for type-ahead.
public class SuggestHeadwordsQuery(string prefix, IEnumerable<long> dictionaryIds, int limit) : IQuery<IEnumerable<string>>
{
    public const int MaxLimit = 100;

    public string Prefix { get; } = prefix;
    public IEnumerable<long> DictionaryIds { get; } = dictionaryIds;
    public int Limit { get; } = limit;
}

public class SuggestHeadwordsQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<SuggestHeadwordsQuery, IEnumerable<string>>
{
    public override async Task<IEnumerable<string>> ExecuteAsync(SuggestHeadwordsQuery query, CancellationToken cancellationToken = default)
    {
        var prefix = WordNormaliser.Normalise(query.Prefix);
        if (prefix.Length == 0) throw new BadRequestException("A prefix is required.");

        var limit = Math.Clamp(query.Limit, 1, SuggestHeadwordsQuery.MaxLimit);
        return await dictionaryRepository.SuggestHeadwords(prefix, query.DictionaryIds, limit, cancellationToken);
    }
}

// The whole dictionary as a zip in the requested format; null when the dictionary does not exist.
public class ExportDictionaryQuery(long dictionaryId, DictionaryExportFormat format) : IQuery<ExportedFileModel>
{
    public long DictionaryId { get; } = dictionaryId;
    public DictionaryExportFormat Format { get; } = format;
}

public class ExportDictionaryQueryHandler(IDictionaryRepository dictionaryRepository, IWriteDictionaryFiles dictionaryFileWriter)
    : QueryHandlerAsync<ExportDictionaryQuery, ExportedFileModel>
{
    public override async Task<ExportedFileModel> ExecuteAsync(ExportDictionaryQuery query, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(query.DictionaryId, cancellationToken);
        if (dictionary == null) return null;

        var articles = await dictionaryRepository.GetAllArticles(query.DictionaryId, cancellationToken);
        return await dictionaryFileWriter.WriteAsync(dictionary, articles, query.Format, cancellationToken);
    }
}

// Groups of articles that look like numbered homographs of one word ("bank 1", "bank 2"), largest
// groups first, as candidates for MergeDictionaryArticlesRequest.
public class GetMergeCandidatesQuery(long dictionaryId, int limit) : IQuery<IEnumerable<MergeCandidateGroupModel>>
{
    public long DictionaryId { get; } = dictionaryId;
    public int Limit { get; } = limit;
}

public class GetMergeCandidatesQueryHandler(IDictionaryRepository dictionaryRepository)
    : QueryHandlerAsync<GetMergeCandidatesQuery, IEnumerable<MergeCandidateGroupModel>>
{
    public override async Task<IEnumerable<MergeCandidateGroupModel>> ExecuteAsync(GetMergeCandidatesQuery query, CancellationToken cancellationToken = default)
    {
        var articles = await dictionaryRepository.GetAllArticles(query.DictionaryId, cancellationToken);

        return articles
            .GroupBy(a => WordNormaliser.Normalise(HeadwordGrouping.BaseHeadword(a.Headword)))
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .Take(Math.Clamp(query.Limit, 1, 1000))
            .Select(g => new MergeCandidateGroupModel
            {
                BaseHeadword = HeadwordGrouping.BaseHeadword(g.First().Headword),
                Articles = g.ToList()
            })
            .ToList();
    }
}
