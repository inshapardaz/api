using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookRatingRequest(int libraryId, int accountId, int bookId) : BookRequest(libraryId, bookId)
{
    public int AccountId { get; } = accountId;
}

public class DeleteBookRatingRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<DeleteBookRatingRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<DeleteBookRatingRequest> HandleAsync(DeleteBookRatingRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await bookRepository.DeleteBookRating(command.LibraryId, command.AccountId, command.BookId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
