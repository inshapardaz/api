using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class RemoveLibraryFromAccountRequest(int libraryId, int accountId) : RequestBase
{
    public int LibraryId { get; } = libraryId;
    public int AccountId { get; } = accountId;
}

public class RemoveLibraryFromAccountRequestHandler(ILibraryRepository libraryRepository)
    : RequestHandlerAsync<RemoveLibraryFromAccountRequest>
{
    [LibraryAuthorize(1, Role.Admin)]
    public override async Task<RemoveLibraryFromAccountRequest> HandleAsync(RemoveLibraryFromAccountRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await libraryRepository.RemoveLibraryFromAccount(command.LibraryId, command.AccountId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
