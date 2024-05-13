using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ChangePasswordCommand : RequestBase
{
    public int? AccountId { get; set; }
    public string Password { get; set; }
    public string OldPassword { get; set; }
}

public class ChangePasswordCommandHandler : RequestHandlerAsync<ChangePasswordCommand>

{
    private readonly IAccountRepository _accountRepository;

    public ChangePasswordCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public override async Task<ChangePasswordCommand> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetAccountById(command.AccountId.Value, cancellationToken);

        if (!SecretHasher.Verify(command.OldPassword, account.PasswordHash))
        {
            throw new BadRequestException();
        }

        account.PasswordHash = SecretHasher.GetStringHash(command.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        await _accountRepository.UpdateAccount(account, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
