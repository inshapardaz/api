using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddLibraryRequest : RequestBase
    {
        public AddLibraryRequest(LibraryModel library)
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

        public override async Task<AddLibraryRequest> HandleAsync(AddLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _libraryRepository.AddLibrary(command.Library, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
