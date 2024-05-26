using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountsByLibraryQuery : IQuery<Page<AccountModel>>
{
    public GetAccountsByLibraryQuery(int libraryId, int pageNumber, int pageSize)
    {
        LibraryId = libraryId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int LibraryId { get; set; }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public string Query { get; set; }
}

public class GetAccountsByLibraryQueryHandler : QueryHandlerAsync<GetAccountsByLibraryQuery, Page<AccountModel>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsByLibraryQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<Page<AccountModel>> ExecuteAsync(GetAccountsByLibraryQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var accounts = string.IsNullOrWhiteSpace(query.Query)
         ? await _accountRepository.GetAccountsByLibrary(query.LibraryId, query.PageNumber, query.PageSize, cancellationToken)
         : await _accountRepository.FindAccountsByLibrary(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);

        return accounts;
    }
}
