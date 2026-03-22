using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.BookShelf;

public class GetBookShelfQuery(int libraryId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<BookShelfModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public string Query { get; set; }
    public bool OnlyPublic { get; set; }
}

public class GetBookShelfQueryHandler(
    IBookShelfRepository bookShelfRepository,
    IFileRepository fileRepository,
    IUserHelper userHelper)
    : QueryHandlerAsync<GetBookShelfQuery, Page<BookShelfModel>>
{
    public override async Task<Page<BookShelfModel>> ExecuteAsync(GetBookShelfQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var BookShelf = string.IsNullOrWhiteSpace(query.Query)
         ? await bookShelfRepository.GetBookShelves(query.LibraryId, query.OnlyPublic, query.PageNumber, query.PageSize, userHelper.AccountId, cancellationToken)
         : await bookShelfRepository.FindBookShelves(query.LibraryId, query.Query, query.OnlyPublic, query.PageNumber, query.PageSize, userHelper.AccountId, cancellationToken);

        foreach (var author in BookShelf.Data)
        {
            if (author != null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, fileRepository, cancellationToken);
            }
        }

        return BookShelf;
    }
}
