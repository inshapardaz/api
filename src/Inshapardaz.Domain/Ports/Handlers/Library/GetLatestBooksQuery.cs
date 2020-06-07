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
    public class GetLatestBooksQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetLatestBooksQuery(int libraryId, Guid? userId)
            : base(libraryId, userId)
        {
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetLatestBooksQueryHandler : QueryHandlerAsync<GetLatestBooksQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetLatestBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetLatestBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetLatestBooks(command.LibraryId, command.PageNumber, command.PageSize, command.UserId, cancellationToken);
        }
    }
}
