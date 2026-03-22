using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class AddLibraryToAccountRequest(int libraryId, int accountId, Role role) : RequestBase
{
    public int LibraryId { get; } = libraryId;
    public int AccountId { get; } = accountId;
    public Role Role { get; } = role;
}

public class AddLibraryToAccountRequestHandler(ILibraryRepository libraryRepository)
    : RequestHandlerAsync<AddLibraryToAccountRequest>
{
    [LibraryAuthorize(1, Role.Admin)]
    public override async Task<AddLibraryToAccountRequest> HandleAsync(AddLibraryToAccountRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await libraryRepository.AddAccountToLibrary(command.LibraryId, command.AccountId, command.Role, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
