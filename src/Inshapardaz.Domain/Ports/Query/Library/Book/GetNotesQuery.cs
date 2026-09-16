using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetNotesQuery(int libraryId, int accountId, int bookId) : LibraryBaseQuery<IEnumerable<NoteModel>>(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
}

public class GetNotesQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetNotesQuery, IEnumerable<NoteModel>>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<IEnumerable<NoteModel>> ExecuteAsync(GetNotesQuery query, CancellationToken cancellationToken = new CancellationToken()) =>
        await bookRepository.GetNotes(query.LibraryId, query.AccountId, query.BookId, cancellationToken);
}
