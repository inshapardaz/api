using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookFromFavoriteRequest(int libraryId, int bookId, int? accountId) : BookRequest(libraryId, bookId)
{
    public int? AccountId { get; } = accountId;
}

public class DeleteBookFromFavoriteRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<DeleteBookFromFavoriteRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<DeleteBookFromFavoriteRequest> HandleAsync(DeleteBookFromFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await bookRepository.DeleteBookFromFavorites(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
