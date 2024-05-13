using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Author;

public class GetAuthorsQuery : LibraryBaseQuery<Page<AuthorModel>>
{
    public GetAuthorsQuery(int libraryId, int pageNumber, int pageSize)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public string Query { get; set; }

    public AuthorTypes? AuthorType { get; set; }
}

public class GetAuthorsQueryHandler : QueryHandlerAsync<GetAuthorsQuery, Page<AuthorModel>>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IFileRepository _fileRepository;

    public GetAuthorsQueryHandler(IAuthorRepository authorRepository, IFileRepository fileRepository)
    {
        _authorRepository = authorRepository;
        _fileRepository = fileRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<Page<AuthorModel>> ExecuteAsync(GetAuthorsQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var authors = string.IsNullOrWhiteSpace(query.Query)
         ? await _authorRepository.GetAuthors(query.LibraryId, query.AuthorType, query.PageNumber, query.PageSize, cancellationToken)
         : await _authorRepository.FindAuthors(query.LibraryId, query.Query, query.AuthorType, query.PageNumber, query.PageSize, cancellationToken);

        foreach (var author in authors.Data)
        {
            if (author != null && author.ImageUrl == null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
            }
        }

        return authors;
    }
}
