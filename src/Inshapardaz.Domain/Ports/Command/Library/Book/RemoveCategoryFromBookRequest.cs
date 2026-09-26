using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class RemoveCategoryFromBookRequest(int libraryId, int bookId, int categoryId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public int CategoryId { get; } = categoryId;
}

public class RemoveCategoryFromBookRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<RemoveCategoryFromBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<RemoveCategoryFromBookRequest> HandleAsync(RemoveCategoryFromBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await bookRepository.RemoveCategoryFromBook(command.LibraryId, command.BookId, command.CategoryId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
