using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Common;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class PasswordResetCommand(string email) : RequestBase
{
    public string Email { get; } = email;
}

public class PasswordResetCommandHandler(
    IAccountRepository accountRepository,
    IOptions<Settings> settings,
    IGenerateToken tokenGenerator,
    ISendEmail emailService)
    : RequestHandlerAsync<PasswordResetCommand>

{
    private readonly Settings _settings = settings.Value;

    public override async Task<PasswordResetCommand> HandleAsync(PasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountByEmail(command.Email, cancellationToken);

        if (account == null)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var resetToken = tokenGenerator.GenerateResetToken();

        account.ResetToken = resetToken;
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(_settings.Security.ResetTokenTTLInDays);

        await accountRepository.UpdateAccount(account, cancellationToken);

        var resetLink = new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath + resetToken).ToString();
        await emailService.SendAsync(account.Email,
                $"Reset your Password",
                EmailTemplateProvider.GetResetPasswordEmail(account.Name, resetLink),
                _settings.Email.EmailFrom,
                cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
