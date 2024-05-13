using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library
{
    public class UpdateLibraryRequest : LibraryBaseCommand
    {
        public UpdateLibraryRequest(int libraryId, LibraryModel library)
            : base(libraryId)
        {
            Library = library;
        }

        public LibraryModel Library { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public LibraryModel Library { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateLibraryRequestHandler : RequestHandlerAsync<UpdateLibraryRequest>
    {
        private readonly ILibraryRepository _libraryRepository;

        public UpdateLibraryRequestHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
        public override async Task<UpdateLibraryRequest> HandleAsync(UpdateLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

            if (result == null)
            {
                var library = command.Library;
                library.Id = default;
                command.Result.Library = await _libraryRepository.AddLibrary(library, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Library.Id = command.LibraryId;
                await _libraryRepository.UpdateLibrary(command.Library, cancellationToken);
                command.Result.Library = command.Library;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
