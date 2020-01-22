using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetAuthorsQuery : IQuery<Page<AuthorModel>>
    {
        public GetAuthorsQuery(int pageNumber, int pageSize)
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
        private readonly IBookRepository _bookRepository;

        public GetAuthorsQueryHandler(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<Page<AuthorModel>> ExecuteAsync(GetAuthorsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Page<AuthorModel> result = null;
            if (string.IsNullOrWhiteSpace(query.Query))
            {
                result = await _authorRepository.GetAuthors(query.PageNumber, query.PageSize, cancellationToken);
            }
            else
            {
                result = await _authorRepository.FindAuthors(query.Query, query.PageNumber, query.PageSize, cancellationToken);
            }

            foreach (var author in result.Data)
            {
                author.BookCount = await _bookRepository.GetBookCountByAuthor(author.Id, cancellationToken);
            }

            return result;
        }
    }
}
