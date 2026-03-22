using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class DeleteBookPageRequest(int libraryId, int bookId, int sequenceNumber) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;

    public int SequenceNumber { get; } = sequenceNumber;
}

public class DeleteBookPageRequestHandler(
    IBookPageRepository bookPageRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteBookPageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookPageRequest> HandleAsync(DeleteBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (bookPage != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(bookPage.ImageId), cancellationToken: cancellationToken);
            await commandProcessor.SendAsync(new DeleteFileCommand(bookPage.ContentId), cancellationToken: cancellationToken);
            await bookPageRepository.DeletePage(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
