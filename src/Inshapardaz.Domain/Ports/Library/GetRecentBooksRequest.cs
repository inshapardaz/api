using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetRecentBooksRequest : RequestBase
    {
        public GetRecentBooksRequest(Guid userId, int count)
        {
            UserId = userId;
            Count = count;
        }

        public Guid UserId {get; }

        public int Count { get; }

        public IEnumerable<Book> Result { get; set; }
    }

    public class GetRecentBooksRequestHandler : RequestHandlerAsync<GetRecentBooksRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetRecentBooksRequestHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetRecentBooksRequest> HandleAsync(GetRecentBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.UserId == Guid.Empty)
            {
                throw new NotFoundException();
            }

            var books = await _bookRepository.GetRecentBooksByUser(command.UserId, command.Count, cancellationToken);
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
