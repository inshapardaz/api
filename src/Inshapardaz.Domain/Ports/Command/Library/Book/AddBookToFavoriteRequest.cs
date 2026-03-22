using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class AddBookToFavoriteRequest(int libraryId, int bookId, int? accountId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public int? AccountId { get; } = accountId;
}

public class AddBookToFavoriteRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<AddBookToFavoriteRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<AddBookToFavoriteRequest> HandleAsync(AddBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            await bookRepository.AddBookToFavorites(command.LibraryId, command.AccountId, command.BookId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
