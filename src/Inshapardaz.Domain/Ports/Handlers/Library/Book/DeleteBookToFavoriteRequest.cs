using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book
{
    public class DeleteBookToFavoriteRequest : BookRequest
    {
        public DeleteBookToFavoriteRequest(int libraryId, int bookId, int? accountId)
            : base(libraryId, bookId)
        {
            AccountId = accountId;
        }

        public int? AccountId { get; }
    }

    public class DeleteBookToFavoriteRequestHandler : RequestHandlerAsync<DeleteBookToFavoriteRequest>
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookToFavoriteRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<DeleteBookToFavoriteRequest> HandleAsync(DeleteBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _bookRepository.DeleteBookFromFavorites(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
