using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteLibraryRequest : LibraryAuthorisedCommand
    {
        public DeleteLibraryRequest(ClaimsPrincipal claims, int libraryId)
            : base(claims, libraryId)
        {
        }
    }

    public class DeleteLibraryRequestHandler : RequestHandlerAsync<DeleteLibraryRequest>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStore;

        public DeleteLibraryRequestHandler(ILibraryRepository libraryRepository, IFileRepository fileRepository, IFileStorage fileStore)
        {
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
            _fileStore = fileStore;
        }

        [Authorise(step: 0, HandlerTiming.Before, Permission.Admin)]
        public override async Task<DeleteLibraryRequest> HandleAsync(DeleteLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library != null)
            {
                await _libraryRepository.DeleteLibrary(command.LibraryId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
