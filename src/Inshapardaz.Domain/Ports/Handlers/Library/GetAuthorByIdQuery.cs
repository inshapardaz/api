using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetAuthorByIdQuery : IQuery<AuthorModel>
    {
        public GetAuthorByIdQuery(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }
    }

    public class GetAuthorByIdQueryHandler : QueryHandlerAsync<GetAuthorByIdQuery, AuthorModel>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<AuthorModel> ExecuteAsync(GetAuthorByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _authorRepository.GetAuthorById(query.AuthorId, cancellationToken);

            if (author != null)
            {
                author.BookCount = await _bookRepository.GetBookCountByAuthor(query.AuthorId, cancellationToken);
            }

            return author;
        }
    }
}
