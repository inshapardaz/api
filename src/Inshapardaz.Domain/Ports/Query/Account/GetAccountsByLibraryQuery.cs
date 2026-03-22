using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountsByLibraryQuery(int libraryId, int pageNumber, int pageSize) : IQuery<Page<AccountModel>>
{
    public int LibraryId { get; set; } = libraryId;

    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; }
}

public class GetAccountsByLibraryQueryHandler(IAccountRepository accountRepository)
    : QueryHandlerAsync<GetAccountsByLibraryQuery, Page<AccountModel>>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<Page<AccountModel>> ExecuteAsync(GetAccountsByLibraryQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var accounts = string.IsNullOrWhiteSpace(query.Query)
         ? await accountRepository.GetAccountsByLibrary(query.LibraryId, query.PageNumber, query.PageSize, cancellationToken)
         : await accountRepository.FindAccountsByLibrary(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);

        return accounts;
    }
}
