using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ResendInvitationCodeCommand(string email) : RequestBase
{
    public string Email { get; } = email;
}

public class ResendInvitationCodeCommandHandler(
    ILibraryRepository libraryRepository,
    IAccountRepository accountRepository,
    ISendEmail emailService,
    IOptions<Settings> settings)
    : RequestHandlerAsync<ResendInvitationCodeCommand>

{
    private readonly Settings _settings = settings.Value;

    public override async Task<ResendInvitationCodeCommand> HandleAsync(ResendInvitationCodeCommand command, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountByEmail(command.Email, cancellationToken);

        if (account != null && !string.IsNullOrWhiteSpace(account.InvitationCode))
        {
            var library = await libraryRepository.GetLibrariesByAccountId(account.Id, cancellationToken);

            // TODO : Read app name from settings
            var libraryName = library.FirstOrDefault()?.Name ?? "Nawishta";

            var invitationCode = Guid.NewGuid();

            await accountRepository.UpdateInvitationCode(
                command.Email,
                invitationCode.ToString("N"),
                DateTime.Today.AddDays(+7),
                cancellationToken);

            await emailService.SendAsync(account.Email,
                $"Welcome to {libraryName}",
                EmailTemplateProvider.GetLibraryUserInvitationEmail(account.Name, libraryName, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                _settings.Email.EmailFrom,
                cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
