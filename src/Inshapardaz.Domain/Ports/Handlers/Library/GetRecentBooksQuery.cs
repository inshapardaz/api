using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetRecentBooksQuery : LibraryBaseQuery<IEnumerable<BookModel>>
    {
        public GetRecentBooksQuery(int libraryId, Guid userId, int count)
            : base(libraryId)
        {
            UserId = userId;
            Count = count;
        }

        public Guid UserId { get; }

        public int Count { get; }
    }

    public class GetRecentBooksQueryHandler : QueryHandlerAsync<GetRecentBooksQuery, IEnumerable<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetRecentBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<IEnumerable<BookModel>> ExecuteAsync(GetRecentBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetRecentBooksByUser(command.LibraryId, command.UserId, command.Count, cancellationToken);
        }
    }
}
