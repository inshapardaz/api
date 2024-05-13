using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.BookShelf;

public class GetBookShelfQuery : LibraryBaseQuery<Page<BookShelfModel>>
{
    public GetBookShelfQuery(int libraryId, int pageNumber, int pageSize)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public string Query { get; set; }
    public bool OnlyPublic { get; set; }
}

public class GetBookShelfQueryHandler : QueryHandlerAsync<GetBookShelfQuery, Page<BookShelfModel>>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IUserHelper _userHelper;

    public GetBookShelfQueryHandler(IBookShelfRepository bookShelfRepository, IFileRepository fileRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _fileRepository = fileRepository;
        _userHelper = userHelper;
    }

    public override async Task<Page<BookShelfModel>> ExecuteAsync(GetBookShelfQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var BookShelf = string.IsNullOrWhiteSpace(query.Query)
         ? await _bookShelfRepository.GetBookShelves(query.LibraryId, query.OnlyPublic, query.PageNumber, query.PageSize, _userHelper.AccountId, cancellationToken)
         : await _bookShelfRepository.FindBookShelves(query.LibraryId, query.Query, query.OnlyPublic, query.PageNumber, query.PageSize, _userHelper.AccountId, cancellationToken);

        foreach (var author in BookShelf.Data)
        {
            if (author != null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
            }
        }

        return BookShelf;
    }
}
