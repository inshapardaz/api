using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Page;

public class GetBookPagesForUserQuery(int libraryId, int accountId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<BookPageModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public EditingStatus StatusFilter { get; set; }
    public int AccountId { get; set; } = accountId;
}

public class GetBookPagesForUserQueryHandler(IBookPageRepository bookPageRepository)
    : QueryHandlerAsync<GetBookPagesForUserQuery, Page<BookPageModel>>
{
    public override async Task<Page<BookPageModel>> ExecuteAsync(GetBookPagesForUserQuery query, CancellationToken cancellationToken = new CancellationToken()) => await bookPageRepository.GetPagesByUser(query.LibraryId, query.AccountId, query.StatusFilter, query.PageNumber, query.PageSize, cancellationToken);
}
