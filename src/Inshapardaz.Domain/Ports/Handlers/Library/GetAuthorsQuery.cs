using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
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
    }

    public class GetAuthorsQueryHandler : QueryHandlerAsync<GetAuthorsQuery, Page<AuthorModel>>
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorsQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public override async Task<Page<AuthorModel>> ExecuteAsync(GetAuthorsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return (string.IsNullOrWhiteSpace(query.Query))
             ? await _authorRepository.GetAuthors(query.LibraryId, query.PageNumber, query.PageSize, cancellationToken)
             : await _authorRepository.FindAuthors(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
