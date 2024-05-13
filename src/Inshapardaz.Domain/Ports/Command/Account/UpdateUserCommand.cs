using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.Extensions.Options;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Account;

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
    private readonly ISendEmail _emailService;
    private readonly Settings _settings;

    public UpdateUserCommandHandler(IAccountRepository accountRepository, ILibraryRepository libraryRepository, ISendEmail emailService, IOptions<Settings> settings)
    {
        _accountRepository = accountRepository;
        _libraryRepository = libraryRepository;
        _emailService = emailService;
        _settings = settings.Value;
    }

    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
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

            await _libraryRepository.UpdateLibraryUser(new LibraryUserModel
            {
                LibraryId = command.LibraryId.Value,
                AccountId = account.Id,
                Role = command.Role
            }, cancellationToken);

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
