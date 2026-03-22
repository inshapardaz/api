using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Page;

public class GetBookPagesQuery(int libraryId, int bookId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<BookPageModel>>(libraryId)
{
    public int BookId { get; private set; } = bookId;

    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public EditingStatus StatusFilter { get; set; }
    public AssignmentFilter AssignmentFilter { get; set; }
    public int? AccountId { get; set; }
    public AssignmentFilter ReviewerAssignmentFilter { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetBookPagesQueryHandler(
    IBookPageRepository bookPageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : QueryHandlerAsync<GetBookPagesQuery, Page<BookPageModel>>
{
    public override async Task<Page<BookPageModel>> ExecuteAsync(GetBookPagesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await bookPageRepository.GetPagesByBook(query.LibraryId, query.BookId, query.PageNumber, query.PageSize, query.StatusFilter, query.AssignmentFilter, query.ReviewerAssignmentFilter, query.SortDirection, query.AccountId, cancellationToken);

        foreach (var page in pages.Data)
        {
            if (page.ContentId.HasValue)
            {
                var file = await fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
        }
        return pages;
    }
}
