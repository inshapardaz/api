using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class DeleteBookFromBookShelfRequest(int libraryId, int bookShelfId, int bookId) : LibraryBaseCommand(libraryId)
{
    public int BookShelfId { get; } = bookShelfId;
    public int BookId { get; } = bookId;
}

public class DeleteBookFromBookShelfRequestHandler(
    IBookShelfRepository bookShelfRepository,
    IBookRepository bookRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<DeleteBookFromBookShelfRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<DeleteBookFromBookShelfRequest> HandleAsync(DeleteBookFromBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        var bookShelf = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (book == null || bookShelf == null)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        if (bookShelf.AccountId != userHelper.AccountId)
        {
            throw new UnauthorizedException();
        }

        await bookShelfRepository.RemoveBookFromBookShelf(command.LibraryId, command.BookShelfId, command.BookId, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
