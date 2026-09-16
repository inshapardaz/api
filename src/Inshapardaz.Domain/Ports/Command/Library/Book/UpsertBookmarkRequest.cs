using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpsertBookmarkRequest(int libraryId, int accountId, int bookId, string clientId, BookmarkModel bookmark)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
    public string ClientId { get; } = clientId;
    public BookmarkModel Bookmark { get; } = bookmark;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public BookmarkModel Bookmark { get; set; }
    }
}

public class UpsertBookmarkRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<UpsertBookmarkRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpsertBookmarkRequest> HandleAsync(UpsertBookmarkRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

        if (book != null)
        {
            var result = await bookRepository.UpsertBookmark(
                command.LibraryId, command.AccountId, command.BookId, command.ClientId, command.Bookmark, cancellationToken);
            command.Result = new UpsertBookmarkRequest.RequestResult
            {
                Bookmark = result
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
