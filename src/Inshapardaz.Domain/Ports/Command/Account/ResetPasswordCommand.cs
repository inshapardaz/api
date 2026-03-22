using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ResetPasswordCommand : RequestBase
{
    public string Token { get; set; }

    public string Password { get; set; }
}

public class ResetPasswordCommandHandler(IAccountRepository accountRepository)
    : RequestHandlerAsync<ResetPasswordCommand>

{
    public override async Task<ResetPasswordCommand> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountByResetToken(command.Token, cancellationToken);

        if (account == null || account.ResetTokenExpires < DateTime.UtcNow)
        {
            throw new BadRequestException("Invitation has expired");
        }

        account.PasswordHash = SecretHasher.GetStringHash(command.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        await accountRepository.UpdateAccount(account, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
