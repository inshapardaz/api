using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetPublishersQuery(int libraryId, string query, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<string>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; } = query;
}

public class GetPublishersQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetPublishersQuery, Page<string>>
{
    public override async Task<Page<string>> ExecuteAsync(GetPublishersQuery query, CancellationToken cancellationToken = default) => await bookRepository.FindPublishers(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);
}
