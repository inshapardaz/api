using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class DeleteBookSelfRequest(int libraryId, int bookShelfId) : LibraryBaseCommand(libraryId)
{
    public int BookShelfId { get; } = bookShelfId;
}

public class DeleteBookSelfRequestHandler(
    IAmACommandProcessor commandProcessor,
    IBookShelfRepository bookShelfRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<DeleteBookSelfRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<DeleteBookSelfRequest> HandleAsync(DeleteBookSelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookShelf = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf != null)
        {
            if (bookShelf.AccountId != userHelper.AccountId)
            {
                throw new ForbiddenException();
            }
            await commandProcessor.SendAsync(new DeleteFileCommand(bookShelf.ImageId), cancellationToken: cancellationToken);
            await bookShelfRepository.DeleteBookShelf(command.LibraryId, command.BookShelfId, cancellationToken);
        }


        return await base.HandleAsync(command, cancellationToken);
    }
}
