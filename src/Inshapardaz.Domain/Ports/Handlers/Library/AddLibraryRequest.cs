using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddLibraryRequest : RequestBase
    {
        public AddLibraryRequest(ClaimsPrincipal claims, LibraryModel library)
        {
            Library = library;
        }

        public LibraryModel Library { get; }

        public LibraryModel Result { get; set; }
    }

    public class AddLibraryRequestHandler : RequestHandlerAsync<AddLibraryRequest>
    {
        private readonly ILibraryRepository _libraryRepository;

        public AddLibraryRequestHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin)]
        public override async Task<AddLibraryRequest> HandleAsync(AddLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _libraryRepository.AddLibrary(command.Library, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
