using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountByIdQuery(int userId) : IQuery<AccountModel>
{
    public int? LibraryId { get; set; }
    public int UserId { get; private set; } = userId;
}

public class GetAccountByIdQueryHandler(
    IAccountRepository accountRepository,
    IUserHelper userHelper)
    : QueryHandlerAsync<GetAccountByIdQuery, AccountModel>
{
    // [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<AccountModel> ExecuteAsync(GetAccountByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        if (query.UserId != userHelper.AccountId && !userHelper.IsAdmin)
            throw new UnauthorizedException();

        if (query.LibraryId.HasValue)
        {
            return await accountRepository.GetLibraryAccountById(query.LibraryId.Value, query.UserId, cancellationToken);
        }
        return await accountRepository.GetAccountById(query.UserId, cancellationToken);
    }
}
