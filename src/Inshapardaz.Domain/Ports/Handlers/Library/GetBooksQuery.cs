using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetBooksQuery(int libraryId, int pageNumber, int pageSize, Guid userId)
            : base(libraryId, userId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetBooksQueryHandler : QueryHandlerAsync<GetBooksQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return (string.IsNullOrWhiteSpace(command.Query))
             ? await _bookRepository.GetBooks(command.LibraryId, command.PageNumber, command.PageSize, command.UserId, cancellationToken)
             : await _bookRepository.SearchBooks(command.LibraryId, command.Query, command.PageNumber, command.PageSize, command.UserId, cancellationToken);
        }
    }
}
