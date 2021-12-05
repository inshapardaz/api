using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class UpdateUserCommand : RequestBase
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int? LibraryId { get; set; }
        public Role Role { get; set; }
    }

    public class UpdateUserCommandHandler : RequestHandlerAsync<UpdateUserCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IEmailService _emailService;
        private readonly Settings _settings;

        public UpdateUserCommandHandler(IAccountRepository accountRepository, ILibraryRepository libraryRepository, IEmailService emailService, Settings settings)
        {
            _accountRepository = accountRepository;
            _libraryRepository = libraryRepository;
            _emailService = emailService;
            _settings = settings;
        }

        public override async Task<UpdateUserCommand> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetAccountByEmail(command.Email, cancellationToken);

            if (account != null && command.LibraryId.HasValue)
            {

                var library = await _libraryRepository.GetLibraryById(command.LibraryId.Value, cancellationToken);

                if (command.Role != Role.Admin && library == null)
                {
                    throw new UnauthorizedException();
                }

                await _libraryRepository.UpdateLibraryUser(new LibraryUserModel { 
                    LibraryId = command.LibraryId.Value,
                    AccountId = account.Id,
                    Role = command.Role}, cancellationToken);
                
                return await base.HandleAsync(command, cancellationToken);
            }

            if (!command.LibraryId.HasValue) 
            {
                if (account != null)
                {
                    account.Name = command.Name;
                    await _accountRepository.UpdateAccount(account, cancellationToken);
                    return await base.HandleAsync(command, cancellationToken);
                }
            }

            throw new BadRequestException();
        }
    }
}
