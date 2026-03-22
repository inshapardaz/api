using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class UpdateUserCommand(int userId) : RequestBase
{
    public string Email { get; set; }
    public string Name { get; set; }
    public int? LibraryId { get; set; }
    public Role Role { get; set; }
    public int UserId { get; set; } = userId;
}

public class UpdateUserCommandHandler(
    IAccountRepository accountRepository,
    ILibraryRepository libraryRepository,
    ISendEmail emailService,
    IOptions<Settings> settings,
    IUserHelper userHelper)
    : RequestHandlerAsync<UpdateUserCommand>

{
    private readonly ISendEmail _emailService = emailService;
    private readonly Settings _settings = settings.Value;

    [Authorize(1)]
    public override async Task<UpdateUserCommand> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        if (command.UserId != userHelper.AccountId)
            throw new UnauthorizedException();

        var account = await accountRepository.GetAccountByEmail(command.Email, cancellationToken);

        if (account != null && command.LibraryId.HasValue)
        {

            var library = await libraryRepository.GetLibraryById(command.LibraryId.Value, cancellationToken);

            if (command.Role != Role.Admin && library == null)
            {
                throw new UnauthorizedException();
            }

            await libraryRepository.UpdateLibraryUser(new LibraryUserModel
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
                await accountRepository.UpdateAccount(account, cancellationToken);
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        throw new BadRequestException();
    }
}
