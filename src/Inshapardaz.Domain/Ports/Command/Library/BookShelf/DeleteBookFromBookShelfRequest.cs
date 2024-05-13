using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class DeleteBookFromBookShelfRequest : LibraryBaseCommand
{
    public DeleteBookFromBookShelfRequest(int libraryId, int bookShelfId, int bookId)
        : base(libraryId)
    {
        BookShelfId = bookShelfId;
        BookId = bookId;
    }

    public int BookShelfId { get; }
    public int BookId { get; }
}

public class DeleteBookFromBookShelfRequestHandler : RequestHandlerAsync<DeleteBookFromBookShelfRequest>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserHelper _userHelper;

    public DeleteBookFromBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IBookRepository bookRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _bookRepository = bookRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1)]
    public override async Task<DeleteBookFromBookShelfRequest> HandleAsync(DeleteBookFromBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (book == null || bookShelf == null)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        if (bookShelf.AccountId != _userHelper.AccountId)
        {
            throw new UnauthorizedException();
        }

        await _bookShelfRepository.RemoveBookFromBookShelf(command.LibraryId, command.BookShelfId, command.BookId, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
