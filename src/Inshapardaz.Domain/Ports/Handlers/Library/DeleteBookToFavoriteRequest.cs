using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteBookToFavoriteRequest : RequestBase
    {
        public DeleteBookToFavoriteRequest(int bookId, Guid user)
        {
            BookId = bookId;
            User = user;
        }

        public int BookId { get; }

        public Guid User { get; }
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
            await _bookRepository.AddBookToFavorites(command.User, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
