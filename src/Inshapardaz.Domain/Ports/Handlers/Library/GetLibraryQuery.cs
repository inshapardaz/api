using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Handlers
{
    public class GetLibraryQuery : LibraryBaseQuery<LibraryModel>
    {
        public GetLibraryQuery(int libraryid, ClaimsPrincipal user)
            : base(libraryid)
        {
            User = user;
        }

        public ClaimsPrincipal User { get; private set; }
    }

    public class GetLibraryQueryHandler : QueryHandlerAsync<GetLibraryQuery, LibraryModel>
    {
        private readonly ILibraryRepository _libraryRepository;

        public GetLibraryQueryHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public override async Task<LibraryModel> ExecuteAsync(GetLibraryQuery query, CancellationToken cancellationToken = default)
        {
            return await _libraryRepository.GetLibraryById(query.LibraryId, cancellationToken);
        }
    }
}
