using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.BookShelf;

public class GetBookShelfByIdQuery(int libraryId, int bookShelfId) : LibraryBaseQuery<BookShelfModel>(libraryId)
{
    public int BookShelfId { get; } = bookShelfId;
}

public class GetBookShelfByIdQueryHandler(
    IBookShelfRepository bookShelfRepository,
    IFileRepository fileRepository,
    IUserHelper userHelper)
    : QueryHandlerAsync<GetBookShelfByIdQuery, BookShelfModel>
{
    public override async Task<BookShelfModel> ExecuteAsync(GetBookShelfByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookShelf = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf != null && !bookShelf.IsPublic && bookShelf.AccountId != userHelper.AccountId)
        {
            throw new NotFoundException();
        }
        if (bookShelf != null && bookShelf.ImageId.HasValue)
        {
            bookShelf.ImageUrl = await ImageHelper.TryConvertToPublicFile(bookShelf.ImageId.Value, fileRepository, cancellationToken);
        }

        return bookShelf;
    }
}
