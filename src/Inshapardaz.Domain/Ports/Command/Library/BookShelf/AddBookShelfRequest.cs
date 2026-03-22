using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class AddBookShelfRequest(int libraryId, BookShelfModel bookShelf) : LibraryBaseCommand(libraryId)
{
    public BookShelfModel BookShelf { get; } = bookShelf;
    public BookShelfModel Result { get; set; }
}

public class AddBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IUserHelper userHelper)
    : RequestHandlerAsync<AddBookShelfRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<AddBookShelfRequest> HandleAsync(AddBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.BookShelf.AccountId = userHelper.AccountId.Value;
        command.Result = await bookShelfRepository.AddBookShelf(command.LibraryId, command.BookShelf, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
