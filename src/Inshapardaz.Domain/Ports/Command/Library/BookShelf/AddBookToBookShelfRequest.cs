using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class AddBookToBookShelfRequest(int libraryId, int bookShelfId, int bookId, int index)
    : LibraryBaseCommand(libraryId)
{
    public int BookShelfId { get; } = bookShelfId;
    public int BookId { get; } = bookId;
    public int Index { get; } = index;
}

public class AddBookToBookShelfRequestHandler(
    IBookShelfRepository bookShelfRepository,
    IBookRepository bookRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<AddBookToBookShelfRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<AddBookToBookShelfRequest> HandleAsync(AddBookToBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException("Book does not exist");
        }

        var bookShelf = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf == null)
        {
            throw new BadRequestException("Bookshelf does not exist");
        }

        if (bookShelf.AccountId != userHelper.AccountId)
        {
            throw new ForbiddenException();
        }

        await bookShelfRepository.AddBookToBookShelf(command.LibraryId, command.BookShelfId, command.BookId, command.Index, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
