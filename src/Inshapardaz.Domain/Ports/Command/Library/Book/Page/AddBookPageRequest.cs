using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class AddBookPageRequest : LibraryBaseCommand
{
    public AddBookPageRequest(int libraryId, int bookId, int? accountId, BookPageModel book)
    : base(libraryId)
    {
        AccountId = accountId;
        BookPage = book;
        BookPage.BookId = bookId;
    }

    public int? AccountId { get; }
    public BookPageModel BookPage { get; }

    public BookPageModel Result { get; set; }

    public bool IsAdded { get; set; }
}

public class AddBookPageRequestHandler : RequestHandlerAsync<AddBookPageRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookPageRepository _bookPageRepository;

    public AddBookPageRequestHandler(IBookRepository bookRepository,
                                     IBookPageRepository bookPageRepository)
    {
        _bookRepository = bookRepository;
        _bookPageRepository = bookPageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddBookPageRequest> HandleAsync(AddBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookPage.BookId, command.AccountId, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException();
        }

        var existingBookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, cancellationToken);

        if (existingBookPage == null)
        {

            var pageSequenceNumber = command.BookPage.SequenceNumber == 0 ? book.PageCount + 1 : command.BookPage.SequenceNumber;
            command.Result = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage.BookId, pageSequenceNumber, command.BookPage.Text, 0, command.BookPage.ChapterId, cancellationToken);
            command.IsAdded = true;
        }
        else
        {
            command.Result = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, command.BookPage.Text, 0, command.BookPage.Status, command.BookPage.ChapterId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
