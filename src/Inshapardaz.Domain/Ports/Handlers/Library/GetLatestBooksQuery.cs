using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetLatestBooksQuery : LibraryBaseQuery<Page<BookModel>>
    {
        public GetLatestBooksQuery(int libraryId, int? accountId)
            : base(libraryId)
        {
            AccountId = accountId;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public int? AccountId { get; }
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
            return await _bookRepository.GetLatestBooks(command.LibraryId, command.PageNumber, command.PageSize, command.AccountId, cancellationToken);
        }
    }
}
