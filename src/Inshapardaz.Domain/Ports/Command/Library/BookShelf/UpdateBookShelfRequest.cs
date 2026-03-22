using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class UpdateBookShelfRequest(int libraryId, BookShelfModel bookShelf) : LibraryBaseCommand(libraryId)
{
    public BookShelfModel BookShelf { get; } = bookShelf;

    public UpdateBookShelfResult Result { get; } = new UpdateBookShelfResult();

    public class UpdateBookShelfResult
    {
        public bool HasAddedNew { get; set; }

        public BookShelfModel BookShelf { get; set; }
    }
}

public class UpdateBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IUserHelper userHelper)
    : RequestHandlerAsync<UpdateBookShelfRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<UpdateBookShelfRequest> HandleAsync(UpdateBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelf.Id, cancellationToken);

        if (result != null && result.AccountId != userHelper.AccountId)
        {
            throw new ForbiddenException();
        }

        if (result == null)
        {
            command.BookShelf.Id = default;
            command.BookShelf.AccountId = userHelper.AccountId.Value;
            var newBookShelf = await bookShelfRepository.AddBookShelf(command.LibraryId, command.BookShelf, cancellationToken);
            command.Result.HasAddedNew = true;
            command.Result.BookShelf = newBookShelf;
        }
        else
        {
            result.Name = command.BookShelf.Name;
            result.Description = command.BookShelf.Description;
            result.IsPublic = command.BookShelf.IsPublic;
            await bookShelfRepository.UpdateBookShelf(command.LibraryId, result, cancellationToken);
            command.Result.BookShelf = command.BookShelf;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
