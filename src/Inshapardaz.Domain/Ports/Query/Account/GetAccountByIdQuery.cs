using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetAccountByIdQuery : IQuery<AccountModel>
{
    public GetAccountByIdQuery(int userId)
    {
        UserId = userId;
    }

    public int? LibraryId { get; set; }
    public int UserId { get; private set; }
}

public class GetAccountByIdQueryHandler : QueryHandlerAsync<GetAccountByIdQuery, AccountModel>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserHelper _userHelper;

    public GetAccountByIdQueryHandler(IAccountRepository accountRepository, 
        IUserHelper userHelper)
    {
        _accountRepository = accountRepository;
        _userHelper = userHelper;
    }

    // [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<AccountModel> ExecuteAsync(GetAccountByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        if (query.UserId != _userHelper.AccountId && !_userHelper.IsAdmin)
            throw new UnauthorizedException();

        if (query.LibraryId.HasValue)
        {
            return await _accountRepository.GetLibraryAccountById(query.LibraryId.Value, query.UserId, cancellationToken);
        }
        return await _accountRepository.GetAccountById(query.UserId, cancellationToken);
    }
}
