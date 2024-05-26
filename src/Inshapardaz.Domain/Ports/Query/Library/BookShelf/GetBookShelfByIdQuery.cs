using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.BookShelf;

public class GetBookShelfByIdQuery : LibraryBaseQuery<BookShelfModel>
{
    public GetBookShelfByIdQuery(int libraryId, int bookShelfId)
        : base(libraryId)
    {
        BookShelfId = bookShelfId;
    }

    public int BookShelfId { get; }
}

public class GetBookShelfByIdQueryHandler : QueryHandlerAsync<GetBookShelfByIdQuery, BookShelfModel>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IUserHelper _userHelper;

    public GetBookShelfByIdQueryHandler(IBookShelfRepository BookShelfRepository, IFileRepository fileRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = BookShelfRepository;
        _fileRepository = fileRepository;
        _userHelper = userHelper;
    }

    public override async Task<BookShelfModel> ExecuteAsync(GetBookShelfByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf != null && !bookShelf.IsPublic && bookShelf.AccountId != _userHelper.AccountId)
        {
            throw new NotFoundException();
        }
        if (bookShelf != null && bookShelf.ImageId.HasValue)
        {
            bookShelf.ImageUrl = await ImageHelper.TryConvertToPublicFile(bookShelf.ImageId.Value, _fileRepository, cancellationToken);
        }

        return bookShelf;
    }
}
