using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountsQuery : IQuery<Page<AccountModel>>
{
    public GetAccountsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public string Query { get; set; }
}

public class GetAccountsQueryHandler : QueryHandlerAsync<GetAccountsQuery, Page<AccountModel>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [AuthorizeAdmin(1)]
    public override async Task<Page<AccountModel>> ExecuteAsync(GetAccountsQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var accounts = string.IsNullOrWhiteSpace(query.Query)
         ? await _accountRepository.GetAccounts(query.PageNumber, query.PageSize, cancellationToken)
         : await _accountRepository.FindAccounts(query.Query, query.PageNumber, query.PageSize, cancellationToken);

        return accounts;
    }
}
