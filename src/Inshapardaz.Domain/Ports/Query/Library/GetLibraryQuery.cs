using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetLibraryQuery : LibraryBaseQuery<LibraryModel>
{
    public GetLibraryQuery(int libraryid)
        : base(libraryid)
    {
    }
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
