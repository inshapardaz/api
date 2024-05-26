using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class UpdateBookShelfRequest : LibraryBaseCommand
{
    public UpdateBookShelfRequest(int libraryId, BookShelfModel bookShelf)
        : base(libraryId)
    {
        BookShelf = bookShelf;
    }

    public BookShelfModel BookShelf { get; }

    public UpdateBookShelfResult Result { get; } = new UpdateBookShelfResult();

    public class UpdateBookShelfResult
    {
        public bool HasAddedNew { get; set; }

        public BookShelfModel BookShelf { get; set; }
    }
}

public class UpdateBookShelfRequestHandler : RequestHandlerAsync<UpdateBookShelfRequest>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IUserHelper _userHelper;

    public UpdateBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1)]
    public override async Task<UpdateBookShelfRequest> HandleAsync(UpdateBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelf.Id, cancellationToken);

        if (result != null && result.AccountId != _userHelper.AccountId)
        {
            throw new ForbiddenException();
        }

        if (result == null)
        {
            command.BookShelf.Id = default;
            command.BookShelf.AccountId = _userHelper.AccountId.Value;
            var newBookShelf = await _bookShelfRepository.AddBookShelf(command.LibraryId, command.BookShelf, cancellationToken);
            command.Result.HasAddedNew = true;
            command.Result.BookShelf = newBookShelf;
        }
        else
        {
            result.Name = command.BookShelf.Name;
            result.Description = command.BookShelf.Description;
            result.IsPublic = command.BookShelf.IsPublic;
            await _bookShelfRepository.UpdateBookShelf(command.LibraryId, result, cancellationToken);
            command.Result.BookShelf = command.BookShelf;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
