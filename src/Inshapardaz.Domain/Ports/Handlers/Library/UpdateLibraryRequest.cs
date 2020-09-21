using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateLibraryRequest : LibraryAuthorisedCommand
    {
        public UpdateLibraryRequest(ClaimsPrincipal claims, int libraryId, LibraryModel library)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin)]
        public override async Task<UpdateLibraryRequest> HandleAsync(UpdateLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

            if (result == null)
            {
                var library = command.Library;
                library.Id = default(int);
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
