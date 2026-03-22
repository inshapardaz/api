using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetLibrariesQuery(int pageNumber, int pageSize, int? accountId = null, bool isAdmin = false)
    : IQuery<Page<LibraryModel>>
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public int? AccountId { get; } = accountId;
    public bool IsAdmin { get; } = isAdmin;
    public string Query { get; set; }
}

public class GetLibrariesQueryHandler(ILibraryRepository libraryRepository)
    : QueryHandlerAsync<GetLibrariesQuery, Page<LibraryModel>>
{
    public override async Task<Page<LibraryModel>> ExecuteAsync(GetLibrariesQuery query, CancellationToken cancellationToken = default)
    {
        if (query.AccountId.HasValue)
        {
            if (query.IsAdmin)
            {
                return string.IsNullOrWhiteSpace(query.Query)
                    ? await libraryRepository.GetLibraries(query.PageNumber, query.PageSize, cancellationToken)
                    : await libraryRepository.FindLibraries(query.Query, query.PageNumber, query.PageSize, cancellationToken);
            }
            else
            {
                return string.IsNullOrWhiteSpace(query.Query)
                ? await libraryRepository.GetUserLibraries(query.AccountId.Value, query.PageNumber, query.PageSize, cancellationToken)
                : await libraryRepository.FindUserLibraries(query.Query, query.AccountId.Value, query.PageNumber, query.PageSize, cancellationToken);
            }
        }
        return string.IsNullOrWhiteSpace(query.Query)
            ? await libraryRepository.GetPublicLibraries(query.PageNumber, query.PageSize, cancellationToken)
            : await libraryRepository.FindPublicLibraries(query.Query, query.PageNumber, query.PageSize, cancellationToken);
    }
}
