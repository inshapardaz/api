using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetAuthorsRequest : RequestBase
    {
        public GetAuthorsRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }

        public Page<Author> Result { get; set; }
    }

    public class GetAuthorsRequestHandler : RequestHandlerAsync<GetAuthorsRequest>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public GetAuthorsRequestHandler(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<GetAuthorsRequest> HandleAsync(GetAuthorsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                command.Result = await _authorRepository.GetAuthors(command.PageNumber, command.PageSize, cancellationToken);
            }
            else
            {
                command.Result = await _authorRepository.FindAuthors(command.Query, command.PageNumber, command.PageSize, cancellationToken);
            }

            foreach (var author in command.Result.Data)
            {
                author.BookCount = await _bookRepository.GetBookCountByAuthor(author.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
