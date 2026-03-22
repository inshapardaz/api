using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.Library;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class InviteUserCommand(int libraryId) : LibraryBaseCommand(libraryId)
{
    public string Email { get; set; }
    public string Name { get; set; }
    public Role Role { get; set; }
}

public class InviteUserCommandHandler(
    IAccountRepository accountRepository,
    ILibraryRepository libraryRepository,
    ISendEmail emailService,
    IOptions<Settings> settings)
    : RequestHandlerAsync<InviteUserCommand>

{
    private readonly Settings _settings = settings.Value;

    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<InviteUserCommand> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken = default)
    {
        var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

        if (command.Role != Role.Admin && library == null)
        {
            throw new BadRequestException();
        }

        var account = await accountRepository.GetAccountByEmail(command.Email, cancellationToken);
        if (account != null)
        {
            if (command.Role == Role.Admin && account != null)
            {
                throw new ConflictException();
            }

            var accountLibraries = await libraryRepository.GetLibrariesByAccountId(account.Id, cancellationToken);
            if (accountLibraries.Any(t => t.Id == command.LibraryId))
            {
                if (account.IsVerified)
                {
                    throw new ConflictException();
                }
                else
                {
                    return await base.HandleAsync(command, cancellationToken);
                }

            }
            else
            {
                await libraryRepository.AddAccountToLibrary(command.LibraryId, account.Id, command.Role, cancellationToken);
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        var invitationCode = Guid.NewGuid();

        await accountRepository.AddInvitedAccount(
            command.Name,
            command.Email,
            command.Role,
            invitationCode.ToString("N"),
            DateTime.Today.AddDays(7),
            library?.Id,
            cancellationToken);

        if (command.Role == Role.Admin)
        {
            await emailService.SendAsync(command.Email,
                $"Welcome to Dashboards",
                EmailTemplateProvider.GetSuperAdminInvitationEmail(command.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                _settings.Email.EmailFrom,
                cancellationToken);
        }
        else
        {
            await emailService.SendAsync(command.Email,
                $"Welcome to {library.Name}",
                EmailTemplateProvider.GetLibraryUserInvitationEmail(command.Name, library.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                _settings.Email.EmailFrom,
                cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
