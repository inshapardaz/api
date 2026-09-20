using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class DeleteAccountCommand(int accountId) : RequestBase
{
    public int AccountId { get; } = accountId;
}

public class DeleteAccountCommandHandler(
    IAccountRepository accountRepository,
    IUserHelper userHelper,
    IGetIPAddress ipAddressGetter)
    : RequestHandlerAsync<DeleteAccountCommand>
{
    [Authorize(1)]
    public override async Task<DeleteAccountCommand> HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken = default)
    {
        if (command.AccountId != userHelper.AccountId && !userHelper.IsAdmin)
        {
            throw new UnauthorizedException();
        }

        var account = await accountRepository.GetAccountById(command.AccountId, cancellationToken);
        if (account == null || account.IsDeleted)
        {
            throw new NotFoundException();
        }

        await accountRepository.RevokeAllRefreshTokens(account.Id, ipAddressGetter.GetIPAddressFromRequest(), cancellationToken);
        await accountRepository.AnonymizeAccount(account.Id, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
