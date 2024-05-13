using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class DeleteBookSelfRequest : LibraryBaseCommand
{
    public DeleteBookSelfRequest(int libraryId, int bookShelfId)
        : base(libraryId)
    {
        BookShelfId = bookShelfId;
    }

    public int BookShelfId { get; }
}

public class DeleteBookSelfRequestHandler : RequestHandlerAsync<DeleteBookSelfRequest>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IUserHelper _userHelper;


    public DeleteBookSelfRequestHandler(IBookShelfRepository bookShelfRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1)]
    public override async Task<DeleteBookSelfRequest> HandleAsync(DeleteBookSelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (result != null && result.AccountId != _userHelper.AccountId)
        {
            throw new ForbiddenException();
        }

        await _bookShelfRepository.DeleteBookShelf(command.LibraryId, command.BookShelfId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
