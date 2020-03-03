using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddBookToFavoriteRequest : LibraryAuthorisedCommand
    {
        public AddBookToFavoriteRequest(ClaimsPrincipal claims, int libraryId, int bookId, Guid user)
            : base(claims, libraryId)
        {
            BookId = bookId;
            User = user;
        }

        public int BookId { get; }

        public Guid User { get; }
    }

    public class AddBookToFavoriteRequestHandler : RequestHandlerAsync<AddBookToFavoriteRequest>
    {
        private readonly IBookRepository _bookRepository;

        public AddBookToFavoriteRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddBookToFavoriteRequest> HandleAsync(AddBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _bookRepository.AddBookToFavorites(command.LibraryId, command.User, command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
