using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Transliteration;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.Transliteration;

// Entries whose roman or native form contains Query, in roman order.
public class GetTransliterationEntriesQuery(string language, string query, int pageNumber, int pageSize) : IQuery<Page<TransliterationEntryModel>>
{
    public string Language { get; } = language;
    public string Query { get; } = query;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}

public class GetTransliterationEntriesQueryHandler(ITransliterationRepository transliterationRepository)
    : QueryHandlerAsync<GetTransliterationEntriesQuery, Page<TransliterationEntryModel>>
{
    public override async Task<Page<TransliterationEntryModel>> ExecuteAsync(GetTransliterationEntriesQuery query, CancellationToken cancellationToken = default)
        => await transliterationRepository.GetEntries(query.Language, query.Query?.Trim(), query.PageNumber, query.PageSize, cancellationToken);
}

public class GetTransliterationEntryByIdQuery(string language, long id) : IQuery<TransliterationEntryModel>
{
    public string Language { get; } = language;
    public long Id { get; } = id;
}

public class GetTransliterationEntryByIdQueryHandler(ITransliterationRepository transliterationRepository)
    : QueryHandlerAsync<GetTransliterationEntryByIdQuery, TransliterationEntryModel>
{
    public override async Task<TransliterationEntryModel> ExecuteAsync(GetTransliterationEntryByIdQuery query, CancellationToken cancellationToken = default)
        => await transliterationRepository.GetEntryById(query.Language, query.Id, cancellationToken);
}

public class GetAllTransliterationEntriesQuery(string language) : IQuery<IEnumerable<TransliterationEntryModel>>
{
    public string Language { get; } = language;
}

public class GetAllTransliterationEntriesQueryHandler(ITransliterationRepository transliterationRepository)
    : QueryHandlerAsync<GetAllTransliterationEntriesQuery, IEnumerable<TransliterationEntryModel>>
{
    public override async Task<IEnumerable<TransliterationEntryModel>> ExecuteAsync(GetAllTransliterationEntriesQuery query, CancellationToken cancellationToken = default)
        => await transliterationRepository.GetAllEntries(query.Language, cancellationToken);
}
