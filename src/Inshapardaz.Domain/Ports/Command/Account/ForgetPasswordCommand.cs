using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ForgetPasswordCommand : RequestBase
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string InvitationCode { get; set; }
    public bool AcceptTerms { get; set; }
}

public class ForgetPasswordCommandHandler(IAccountRepository accountRepository)
    : RequestHandlerAsync<ForgetPasswordCommand>

{
    public override async Task<ForgetPasswordCommand> HandleAsync(ForgetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        if (!command.AcceptTerms)
        {
            throw new BadRequestException("Terms must be accepted");
        }

        if (command.Password != command.ConfirmPassword)
        {
            throw new BadRequestException("Password and confirm password not matching");
        }

        var account = await accountRepository.GetAccountByInvitationCode(command.InvitationCode, cancellationToken);

        if (account == null || account.InvitationCodeExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invitation has expired");
        }

        account.Name = command.Name;
        account.PasswordHash = SecretHasher.GetStringHash(command.Password);
        account.Verified = DateTime.UtcNow;
        account.VerificationToken = RandomGenerator.GenerateRandomString();
        account.InvitationCode = null;
        account.InvitationCodeExpiry = null;
        account.AcceptTerms = true;

        await accountRepository.UpdateAccount(account, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
