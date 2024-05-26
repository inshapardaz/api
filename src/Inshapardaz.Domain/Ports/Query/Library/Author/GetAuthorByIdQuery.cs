using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Author;

public class GetAuthorByIdQuery : LibraryBaseQuery<AuthorModel>
{
    public GetAuthorByIdQuery(int libraryId, int authorId)
        : base(libraryId)
    {
        AuthorId = authorId;
    }

    public int AuthorId { get; }
}

public class GetAuthorByIdQueryHandler : QueryHandlerAsync<GetAuthorByIdQuery, AuthorModel>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IFileRepository _fileRepository;

    public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository, IFileRepository fileRepository)
    {
        _authorRepository = authorRepository;
        _fileRepository = fileRepository;
    }

    public override async Task<AuthorModel> ExecuteAsync(GetAuthorByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await _authorRepository.GetAuthorById(query.LibraryId, query.AuthorId, cancellationToken);

        if (author != null && author.ImageUrl == null && author.ImageId.HasValue)
        {
            author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
        }

        return author;
    }
}
