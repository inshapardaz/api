using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library
{
    public class AddLibraryToAccountRequest : RequestBase
    {
        public AddLibraryToAccountRequest(int libraryId, int accountId, Role role)
        {
            LibraryId = libraryId;
            AccountId = accountId;
            Role = role;
        }

        public int LibraryId { get; }
        public int AccountId { get; }
        public Role Role { get; }
    }

    public class AddLibraryToAccountRequestHandler : RequestHandlerAsync<AddLibraryToAccountRequest>
    {
        private readonly ILibraryRepository _libraryRepository;

        public AddLibraryToAccountRequestHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [LibraryAuthorize(1, Role.Admin)]
        public override async Task<AddLibraryToAccountRequest> HandleAsync(AddLibraryToAccountRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _libraryRepository.AddAccountToLibrary(command.LibraryId, command.AccountId, command.Role, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
