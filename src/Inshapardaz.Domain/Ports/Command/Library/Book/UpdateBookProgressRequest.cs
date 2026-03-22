using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookProgressRequest(int libraryId, int accountId, int bookId, ReadProgressModel progress)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
    public ReadProgressModel Progress { get; } = progress;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public ReadProgressModel Progress { get; set; }
    }
}

public class UpdateBookProgressRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<UpdateBookProgressRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateBookProgressRequest> HandleAsync(UpdateBookProgressRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

        if (book != null)
        {
            var response = await bookRepository.AddRecentBook(command.LibraryId, command.AccountId, command.BookId, command.Progress, cancellationToken);
            command.Result = new UpdateBookProgressRequest.RequestResult()
            {
                Progress = response
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
