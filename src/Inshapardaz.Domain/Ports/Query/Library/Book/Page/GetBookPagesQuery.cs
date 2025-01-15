using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Page;

public class GetBookPagesQuery : LibraryBaseQuery<Page<BookPageModel>>
{
    public GetBookPagesQuery(int libraryId, int bookId, int pageNumber, int pageSize)
        : base(libraryId)
    {
        BookId = bookId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int BookId { get; private set; }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public EditingStatus StatusFilter { get; set; }
    public AssignmentFilter AssignmentFilter { get; set; }
    public int? AccountId { get; set; }
    public AssignmentFilter ReviewerAssignmentFilter { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetBookPagesQueryHandler : QueryHandlerAsync<GetBookPagesQuery, Page<BookPageModel>>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetBookPagesQueryHandler(IBookPageRepository bookPageRepository,
        IFileRepository fileRepository,
        IFileStorage fileStorage)
    {
        _bookPageRepository = bookPageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<Page<BookPageModel>> ExecuteAsync(GetBookPagesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await _bookPageRepository.GetPagesByBook(query.LibraryId, query.BookId, query.PageNumber, query.PageSize, query.StatusFilter, query.AssignmentFilter, query.ReviewerAssignmentFilter, query.SortDirection, query.AccountId, cancellationToken);

        foreach (var page in pages.Data)
        {
            if (page.ContentId.HasValue)
            {
                var file = await _fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
        }
        return pages;
    }
}
