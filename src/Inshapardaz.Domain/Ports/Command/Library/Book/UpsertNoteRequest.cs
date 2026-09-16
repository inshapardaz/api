using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpsertNoteRequest(int libraryId, int accountId, int bookId, string clientId, NoteModel note)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
    public string ClientId { get; } = clientId;
    public NoteModel Note { get; } = note;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public NoteModel Note { get; set; }
    }
}

public class UpsertNoteRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<UpsertNoteRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpsertNoteRequest> HandleAsync(UpsertNoteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

        if (book != null)
        {
            var result = await bookRepository.UpsertNote(
                command.LibraryId, command.AccountId, command.BookId, command.ClientId, command.Note, cancellationToken);
            command.Result = new UpsertNoteRequest.RequestResult
            {
                Note = result
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
