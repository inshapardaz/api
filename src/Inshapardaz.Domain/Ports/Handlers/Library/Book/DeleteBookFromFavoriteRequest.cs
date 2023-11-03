using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book
{
    public class DeleteBookFromFavoriteRequest : BookRequest
    {
        public DeleteBookFromFavoriteRequest(int libraryId, int bookId, int? accountId)
            : base(libraryId, bookId)
        {
            AccountId = accountId;
        }

        public int? AccountId { get; }
    }

    public class DeleteBookFromFavoriteRequestHandler : RequestHandlerAsync<DeleteBookFromFavoriteRequest>
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookFromFavoriteRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<DeleteBookFromFavoriteRequest> HandleAsync(DeleteBookFromFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _bookRepository.DeleteBookFromFavorites(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
