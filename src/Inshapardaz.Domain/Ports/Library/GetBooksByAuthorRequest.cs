using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByAuthorRequest : RequestBase
    {
        public GetBooksByAuthorRequest(int authorId, int pageNumber, int pageSize)
        {
            AuthorId = authorId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int AuthorId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; set; }

        public Page<Book> Result { get; set; }
    }

    public class GetBooksByAuthorRequestHandler : RequestHandlerAsync<GetBooksByAuthorRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksByAuthorRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetBooksByAuthorRequest> HandleAsync(GetBooksByAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksByAuthor(command.AuthorId, command.PageNumber, command.PageSize, cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
