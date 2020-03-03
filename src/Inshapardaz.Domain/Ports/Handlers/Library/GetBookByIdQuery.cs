using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBookByIdQuery : LibraryAuthorisedQuery<BookModel>
    {
        public GetBookByIdQuery(int libraryId, int bookId, Guid userId)
            : base(libraryId, userId)
        {
            BookId = bookId;
        }

        public int BookId { get; private set; }
    }

    public class GetBookByIdQueryHandler : QueryHandlerAsync<GetBookByIdQuery, BookModel>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<BookModel> ExecuteAsync(GetBookByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
        }
    }
}
