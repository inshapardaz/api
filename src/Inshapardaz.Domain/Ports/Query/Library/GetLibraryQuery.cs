using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetLibraryQuery(int libraryid) : LibraryBaseQuery<LibraryModel>(libraryid);

public class GetLibraryQueryHandler(ILibraryRepository libraryRepository)
    : QueryHandlerAsync<GetLibraryQuery, LibraryModel>
{
    public override async Task<LibraryModel> ExecuteAsync(GetLibraryQuery query, CancellationToken cancellationToken = default) => await libraryRepository.GetLibraryById(query.LibraryId, cancellationToken);
}
