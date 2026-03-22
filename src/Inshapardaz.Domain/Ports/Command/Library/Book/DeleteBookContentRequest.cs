using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookContentRequest(int libraryId, int bookId, long contentId) : BookRequest(libraryId, bookId)
{
    public long ContentId { get; } = contentId;
}

public class DeleteBookContentRequestHandler(IBookRepository bookRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteBookContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookContentRequest> HandleAsync(DeleteBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        if (content != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(content.FileId), cancellationToken: cancellationToken);
            await bookRepository.DeleteBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
