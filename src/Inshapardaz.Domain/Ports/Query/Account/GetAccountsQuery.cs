using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountsQuery(int pageNumber, int pageSize) : IQuery<Page<AccountModel>>
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; }
}

public class GetAccountsQueryHandler(IAccountRepository accountRepository)
    : QueryHandlerAsync<GetAccountsQuery, Page<AccountModel>>
{
    [AuthorizeAdmin(1)]
    public override async Task<Page<AccountModel>> ExecuteAsync(GetAccountsQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var accounts = string.IsNullOrWhiteSpace(query.Query)
         ? await accountRepository.GetAccounts(query.PageNumber, query.PageSize, cancellationToken)
         : await accountRepository.FindAccounts(query.Query, query.PageNumber, query.PageSize, cancellationToken);

        return accounts;
    }
}
