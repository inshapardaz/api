using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookmarkRequest(int libraryId, int accountId, int bookId, string clientId) : BookRequest(libraryId, bookId)
{
    public int AccountId { get; } = accountId;
    public string ClientId { get; } = clientId;
}

public class DeleteBookmarkRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<DeleteBookmarkRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<DeleteBookmarkRequest> HandleAsync(DeleteBookmarkRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await bookRepository.DeleteBookmark(command.LibraryId, command.AccountId, command.BookId, command.ClientId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
