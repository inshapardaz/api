using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Author;

public class GetAuthorsQuery(int libraryId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<AuthorModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; }

    public AuthorTypes? AuthorType { get; set; }
    public AuthorSortByType SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetAuthorsQueryHandler(IAuthorRepository authorRepository, IFileRepository fileRepository)
    : QueryHandlerAsync<GetAuthorsQuery, Page<AuthorModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<Page<AuthorModel>> ExecuteAsync(GetAuthorsQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var authors = string.IsNullOrWhiteSpace(query.Query)
         ? await authorRepository.GetAuthors(query.LibraryId, query.AuthorType, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken)
         : await authorRepository.FindAuthors(query.LibraryId, query.Query, query.AuthorType, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken);

        foreach (var author in authors.Data)
        {
            if (author != null && author.ImageUrl == null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, fileRepository, cancellationToken);
            }
        }

        return authors;
    }
}
