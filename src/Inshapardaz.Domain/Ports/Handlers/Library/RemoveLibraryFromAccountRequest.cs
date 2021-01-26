using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class RemoveLibraryFromAccountRequest : RequestBase
    {
        public RemoveLibraryFromAccountRequest(int libraryId, int accountId)
        {
            LibraryId = libraryId;
            AccountId = accountId;
        }

        public int LibraryId { get; }
        public int AccountId { get; }
    }

    public class RemoveLibraryFromAccountRequestHandler : RequestHandlerAsync<RemoveLibraryFromAccountRequest>
    {
        private readonly ILibraryRepository _libraryRepository;

        public RemoveLibraryFromAccountRequestHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public override async Task<RemoveLibraryFromAccountRequest> HandleAsync(RemoveLibraryFromAccountRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _libraryRepository.RemoveLibraryFromAccount(command.LibraryId, command.AccountId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
