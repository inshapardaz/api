using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetLatestBooksRequest : RequestBase
    {
        public Guid UserId { get; }

        public GetLatestBooksRequest(Guid userId)
        {
            UserId = userId;
        }

        public IEnumerable<Book> Result { get; set; }
    }

    public class GetLatestBooksRequestHandler : RequestHandlerAsync<GetLatestBooksRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetLatestBooksRequestHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetLatestBooksRequest> HandleAsync(GetLatestBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetLatestBooks(cancellationToken);
            command.Result = await MarkFavorites(books, command.UserId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<IEnumerable<Book>> MarkFavorites(IEnumerable<Book> books, Guid userId, CancellationToken cancellationToken)
        {
            foreach (var book in books)
            {
                book.IsFavorite = await _favoriteRepository.IsBookFavorite(userId, book.Id, cancellationToken);
            }

            return books;
        }
    }
}
