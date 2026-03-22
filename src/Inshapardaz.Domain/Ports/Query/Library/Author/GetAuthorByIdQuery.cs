using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Author;

public class GetAuthorByIdQuery(int libraryId, int authorId) : LibraryBaseQuery<AuthorModel>(libraryId)
{
    public int AuthorId { get; } = authorId;
}

public class GetAuthorByIdQueryHandler(IAuthorRepository authorRepository, IFileRepository fileRepository)
    : QueryHandlerAsync<GetAuthorByIdQuery, AuthorModel>
{
    public override async Task<AuthorModel> ExecuteAsync(GetAuthorByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await authorRepository.GetAuthorById(query.LibraryId, query.AuthorId, cancellationToken);

        if (author != null && author.ImageUrl == null && author.ImageId.HasValue)
        {
            author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, fileRepository, cancellationToken);
        }

        return author;
    }
}
