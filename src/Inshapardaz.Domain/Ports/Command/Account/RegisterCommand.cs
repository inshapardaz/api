using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class RegisterCommand : RequestBase
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string InvitationCode { get; set; }
    public bool AcceptTerms { get; set; }
}

public class RegisterCommandHandler(IAccountRepository accountRepository) : RequestHandlerAsync<RegisterCommand>

{
    public override async Task<RegisterCommand> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        if (!command.AcceptTerms)
        {
            throw new BadRequestException("Terms must be accepted");
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
