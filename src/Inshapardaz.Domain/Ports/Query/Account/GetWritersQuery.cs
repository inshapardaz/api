using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetWritersQuery(int libraryId, string query) : LibraryBaseQuery<IEnumerable<AccountModel>>(libraryId)
{
    public string Query { get; } = query;
}

public class GetWritersQueryHandler(IAccountRepository accountRepository)
    : QueryHandlerAsync<GetWritersQuery, IEnumerable<AccountModel>>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin, Role.Writer)]
    public override async Task<IEnumerable<AccountModel>> ExecuteAsync(GetWritersQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(query.Query))
        {
            return await accountRepository.GetWriters(query.LibraryId, cancellationToken);
        }

        return await accountRepository.FindWriters(query.LibraryId, query.Query, cancellationToken);
    }
}
