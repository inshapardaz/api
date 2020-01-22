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
        private readonly IFavoriteRepository _favoriteRepository;

        public DeleteBookToFavoriteRequestHandler(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<DeleteBookToFavoriteRequest> HandleAsync(DeleteBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _favoriteRepository.AddBookToFavorites(command.User, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
