using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library;
using Microsoft.Extensions.Options;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class UpdateUserCommand : RequestBase
{

    public UpdateUserCommand(int userId)
    {
        UserId = userId;
    }

    public string Email { get; set; }
    public string Name { get; set; }
    public int? LibraryId { get; set; }
    public Role Role { get; set; }
    public int UserId { get; set; }
}

public class UpdateUserCommandHandler : RequestHandlerAsync<UpdateUserCommand>

{
    private readonly IAccountRepository _accountRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly ISendEmail _emailService;
    private readonly Settings _settings;
    private readonly IUserHelper _userHelper;

    public UpdateUserCommandHandler(IAccountRepository accountRepository, ILibraryRepository libraryRepository, ISendEmail emailService, IOptions<Settings> settings, IUserHelper userHelper)
    {
        _accountRepository = accountRepository;
        _libraryRepository = libraryRepository;
        _emailService = emailService;
        _settings = settings.Value;
        _userHelper = userHelper;
    }

    [Authorize(1)]
    public override async Task<UpdateUserCommand> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        if (command.UserId != _userHelper.AccountId)
            throw new UnauthorizedException();

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
