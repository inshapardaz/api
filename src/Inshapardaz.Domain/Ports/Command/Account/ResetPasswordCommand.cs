using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ResetPasswordCommand : RequestBase
{
    public string Token { get; set; }

    public string Password { get; set; }
}

public class ResetPasswordCommandHandler : RequestHandlerAsync<ResetPasswordCommand>

{
    private readonly IAccountRepository _accountRepository;

    public ResetPasswordCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public override async Task<ResetPasswordCommand> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetAccountByResetToken(command.Token, cancellationToken);

        if (account == null || account.ResetTokenExpires < DateTime.UtcNow)
        {
            throw new BadRequestException("Invitation has expired");
        }

        account.PasswordHash = SecretHasher.GetStringHash(command.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        await _accountRepository.UpdateAccount(account, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
