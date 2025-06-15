using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetPublishersQuery : LibraryBaseQuery<Page<string>>
{
    public GetPublishersQuery(int libraryId, string query, int pageNumber, int pageSize) 
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Query = query;
    }

    public int PageNumber { get; private set; }
    
    public int PageSize { get; private set; }
   
    public string Query { get; set; }
}

public class GetPublishersQueryHandler : QueryHandlerAsync<GetPublishersQuery, Page<string>>
{
    private readonly IBookRepository _bookRepository;

    public GetPublishersQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public override async Task<Page<string>> ExecuteAsync(GetPublishersQuery query, CancellationToken cancellationToken = default)
    {
        return await _bookRepository.FindPublishers(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);
    }
}
