using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories;
using Microsoft.Extensions.Options;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class InviteUserCommand : LibraryBaseCommand
    {
        public InviteUserCommand(int libraryId) 
            : base(libraryId)
        {
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
    }

    public class InviteUserCommandHandler : RequestHandlerAsync<InviteUserCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ISendEmail _emailService;
        private readonly Settings _settings;

        public InviteUserCommandHandler(IAccountRepository accountRepository, ILibraryRepository libraryRepository, ISendEmail emailService, IOptions<Settings> settings)
        {
            _accountRepository = accountRepository;
            _libraryRepository = libraryRepository;
            _emailService = emailService;
            _settings = settings.Value;
        }

        [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
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
                    EmailTemplateProvider.GetSuperAdminInvitationEmail(command.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                    _settings.Email.EmailFrom,
                    cancellationToken);
            }
            else
            {
                await _emailService.SendAsync(command.Email,
                    $"Welcome to {library.Name}",
                    EmailTemplateProvider.GetLibraryUserInvitationEmail(command.Name, library.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                    _settings.Email.EmailFrom,
                    cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
