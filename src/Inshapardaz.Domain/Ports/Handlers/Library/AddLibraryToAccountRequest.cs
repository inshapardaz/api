using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddLibraryToAccountRequest : RequestBase
    {
        public AddLibraryToAccountRequest(int libraryId, int accountId)
        {
            LibraryId = libraryId;
            AccountId = accountId;
        }

        public int LibraryId { get; }
        public int AccountId { get; }
    }

    public class AddLibraryToAccountRequestHandler : RequestHandlerAsync<AddLibraryToAccountRequest>
    {
        private readonly ILibraryRepository _libraryRepository;

        public AddLibraryToAccountRequestHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public override async Task<AddLibraryToAccountRequest> HandleAsync(AddLibraryToAccountRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _libraryRepository.AddLibraryToAccount(command.LibraryId, command.AccountId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
