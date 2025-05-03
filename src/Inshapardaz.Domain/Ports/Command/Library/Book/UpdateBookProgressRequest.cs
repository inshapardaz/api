using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookProgressRequest : LibraryBaseCommand
{
    public UpdateBookProgressRequest(int libraryId, int accountId, int bookId, ReadProgressModel progress)
        : base(libraryId)
    {
        AccountId = accountId;
        BookId = bookId;
        Progress = progress;
    }

    public int AccountId { get; }
    public int BookId { get; }
    public ReadProgressModel Progress { get; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public ReadProgressModel Progress { get; set; }
    }
}

public class UpdateBookProgressRequestHandler : RequestHandlerAsync<UpdateBookProgressRequest>
{
    private readonly IBookRepository _bookRepository;

    public UpdateBookProgressRequestHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateBookProgressRequest> HandleAsync(UpdateBookProgressRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

        if (book != null)
        {
            var response = await _bookRepository.AddRecentBook(command.LibraryId, command.AccountId, command.BookId, command.Progress, cancellationToken);
            command.Result = new UpdateBookProgressRequest.RequestResult()
            {
                Progress = response
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
