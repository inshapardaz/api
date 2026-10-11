using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class SetDictionaryWriterCommand(int accountId, bool isDictionaryWriter) : RequestBase
{
    public int AccountId { get; } = accountId;
    public bool IsDictionaryWriter { get; } = isDictionaryWriter;
}

public class SetDictionaryWriterCommandHandler(IAccountRepository accountRepository)
    : RequestHandlerAsync<SetDictionaryWriterCommand>
{
    [AuthorizeAdmin(1)]
    public override async Task<SetDictionaryWriterCommand> HandleAsync(SetDictionaryWriterCommand command, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountById(command.AccountId, cancellationToken);
        if (account == null || account.IsDeleted)
        {
            throw new NotFoundException();
        }

        // Takes effect on the account's next access token (login or refresh).
        await accountRepository.SetDictionaryWriter(command.AccountId, command.IsDictionaryWriter, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
