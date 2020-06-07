using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByAuthorQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetBooksByAuthorQuery(int libraryId, int authorId, int pageNumber, int pageSize, Guid? userId)
            : base(libraryId, userId)
        {
            AuthorId = authorId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int AuthorId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetBooksByAuthorQueryHandler : QueryHandlerAsync<GetBooksByAuthorQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksByAuthorQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksByAuthorQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetBooksByAuthor(command.LibraryId, command.AuthorId, command.PageNumber, command.PageSize, command.UserId, cancellationToken);
        }
    }
}
