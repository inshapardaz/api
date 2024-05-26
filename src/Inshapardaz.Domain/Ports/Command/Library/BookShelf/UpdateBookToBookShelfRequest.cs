using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class UpdateBookToBookShelfRequest : LibraryBaseCommand
{
    public UpdateBookToBookShelfRequest(int libraryId, int bookShelfId, int bookId, int index)
        : base(libraryId)
    {
        BookShelfId = bookShelfId;
        BookId = bookId;
        Index = index;
    }

    public int BookShelfId { get; }
    public int BookId { get; }
    public int Index { get; }
}

public class UpdateBookToBookShelfRequestHandler : RequestHandlerAsync<UpdateBookToBookShelfRequest>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserHelper _userHelper;

    public UpdateBookToBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IBookRepository bookRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _bookRepository = bookRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1)]
    public override async Task<UpdateBookToBookShelfRequest> HandleAsync(UpdateBookToBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException("Book does not exist");
        }

        var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf == null)
        {
            throw new BadRequestException("Bookshelf does not exist");
        }

        if (bookShelf.AccountId != _userHelper.AccountId)
        {
            throw new UnauthorizedException();
        }

        var result = await _bookShelfRepository.GetBookFromBookShelfById(command.LibraryId, command.BookShelfId, command.BookId, cancellationToken);

        if (result == null)
        {
            await _bookShelfRepository.AddBookToBookShelf(command.LibraryId, command.BookShelfId, command.BookId, command.Index, cancellationToken);
        }
        else
        {
            result.Index = command.Index;
            await _bookShelfRepository.UpdateBookToBookShelf(command.LibraryId, result, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
