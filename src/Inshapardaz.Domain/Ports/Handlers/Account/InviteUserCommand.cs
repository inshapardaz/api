using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class InviteUserCommand : RequestBase
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int LibraryId { get; set; }
        public Role Role { get; set; }
    }

    public class InviteUserCommandHandler : RequestHandlerAsync<InviteUserCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IEmailService _emailService;
        private readonly Settings _settings;

        public InviteUserCommandHandler(IAccountRepository accountRepository, ILibraryRepository libraryRepository, IEmailService emailService, Settings settings)
        {
            _accountRepository = accountRepository;
            _libraryRepository = libraryRepository;
            _emailService = emailService;
            _settings = settings;
        }

        public override async Task<InviteUserCommand> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken = default)
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

            if (command.Role != Role.Admin && library == null)
            {
                throw new BadRequestException();
            }

            var account = await _accountRepository.GetAccountByEmail(command.Email, cancellationToken);
            if (account != null)
            {
                if (command.Role == Role.Admin && account != null)
                {
                    throw new ConflictException();
                }

                var accountLibraries = await _libraryRepository.GetLibrariesByAccountId(account.Id, cancellationToken);
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
                    await _libraryRepository.AddAccountToLibrary(command.LibraryId, account.Id, command.Role, cancellationToken);
                    return await base.HandleAsync(command, cancellationToken);
                }
            }

            var invitationCode = Guid.NewGuid();

            await _accountRepository.AddInvitedAccount(
                command.Name,
                command.Email,
                command.Role,
                invitationCode.ToString("N"),
                DateTime.Today.AddDays(7),
                library?.Id,
                cancellationToken);

            if (command.Role == Role.Admin)
            {
                await _emailService.SendAsync(command.Email,
                    $"Welcome to Dashboards",
                    EmailTemplateProvider.GetSuperAdminInvitationEmail(command.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.ResetPasswordPagePath).ToString()),
                    _settings.EmailFrom,
                    cancellationToken);
            }
            else
            {
                await _emailService.SendAsync(command.Email,
                    $"Welcome to {library.Name}",
                    EmailTemplateProvider.GetLibraryUserInvitationEmail(command.Name, library.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.ResetPasswordPagePath).ToString()),
                    _settings.EmailFrom,
                    cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
