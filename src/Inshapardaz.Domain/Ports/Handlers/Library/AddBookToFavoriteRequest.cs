using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddBookToFavoriteRequest : RequestBase
    {
        public AddBookToFavoriteRequest(int bookId, Guid user)
        {
            BookId = bookId;
            User = user;
        }

        public int BookId { get; }

        public Guid User { get; }

    }

    public class AddBookToFavoriteRequestHandler : RequestHandlerAsync<AddBookToFavoriteRequest>
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public AddBookToFavoriteRequestHandler(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<AddBookToFavoriteRequest> HandleAsync(AddBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _favoriteRepository.AddBookToFavorites(command.User, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
