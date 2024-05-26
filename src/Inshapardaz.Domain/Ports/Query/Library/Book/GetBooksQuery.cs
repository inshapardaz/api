using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBooksQuery : LibraryBaseQuery<Page<BookModel>>
{
    public GetBooksQuery(int libraryId, int pageNumber, int pageSize, int? accountId)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        AccountId = accountId;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public int? AccountId { get; }
    public string Query { get; set; }

    public BookSortByType SortBy { get; set; }

    public BookFilter Filter { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetBooksQueryHandler : QueryHandlerAsync<GetBooksQuery, Page<BookModel>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileRepository _fileRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
    {
        _bookRepository = bookRepository;
        _fileRepository = fileRepository;
    }

    public override async Task<Page<BookModel>> ExecuteAsync(GetBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var books = string.IsNullOrWhiteSpace(command.Query)
         ? await _bookRepository.GetBooks(command.LibraryId, command.PageNumber, command.PageSize, command.AccountId, command.Filter, command.SortBy, command.SortDirection, cancellationToken)
         : await _bookRepository.SearchBooks(command.LibraryId, command.Query, command.PageNumber, command.PageSize, command.AccountId, command.Filter, command.SortBy, command.SortDirection, cancellationToken);

        var statuses = await _bookRepository.GetBookPageSummary(command.LibraryId, books.Data.Select(b => b.Id).ToList(), cancellationToken);

        foreach (var status in statuses)
        {
            var book = books.Data.SingleOrDefault(b => b.Id == status.BookId);
            if (book != null)
            {
                book.PageStatus = status.Statuses;
                if (status.Statuses.Any(s => s.Status == EditingStatus.Completed))
                {
                    decimal completedPages = status.Statuses.Single(s => s.Status == EditingStatus.Completed).Count;
                    book.Progress = completedPages / book.PageCount;
                }
                else
                {
                    book.Progress = 0.0M;
                }
            }
        }

        foreach (var book in books.Data)
        {
            if (book != null && book.ImageUrl == null && book.ImageId.HasValue)
            {
                book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, _fileRepository, cancellationToken);
            }

            var contents = await _bookRepository.GetBookContents(command.LibraryId, book.Id, cancellationToken);

            book.Contents = contents.ToList();
        }

        return books;
    }
}
