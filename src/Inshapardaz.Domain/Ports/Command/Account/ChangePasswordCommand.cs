using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ChangePasswordCommand : RequestBase
{
    public string Password { get; set; }
    public string OldPassword { get; set; }
}

public class ChangePasswordCommandHandler(IAccountRepository accountRepository, IUserHelper userHelper)
    : RequestHandlerAsync<ChangePasswordCommand>

{
    [Authorize(1)]
    public override async Task<ChangePasswordCommand> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountById(userHelper.AccountId.Value, cancellationToken);

        if (!SecretHasher.Verify(command.OldPassword, account.PasswordHash))
        {
            throw new BadRequestException();
        }

        account.PasswordHash = SecretHasher.GetStringHash(command.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        await accountRepository.UpdateAccount(account, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
