using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetRecentBooksQuery : IQuery<IEnumerable<BookModel>>
    {
        public GetRecentBooksQuery(Guid userId, int count)
        {
            UserId = userId;
            Count = count;
        }

        public Guid UserId {get; }

        public int Count { get; }
    }

    public class GetRecentBooksQueryHandler : QueryHandlerAsync<GetRecentBooksQuery, IEnumerable<BookModel>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetRecentBooksQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<IEnumerable<BookModel>> ExecuteAsync(GetRecentBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.UserId == Guid.Empty)
            {
                throw new NotFoundException();
            }

            var books = await _bookRepository.GetRecentBooksByUser(command.UserId, command.Count, cancellationToken);
            return await MarkFavorites(books, command.UserId, cancellationToken);
        }

        private async Task<IEnumerable<BookModel>> MarkFavorites(IEnumerable<BookModel> books, Guid userId, CancellationToken cancellationToken)
        {
            foreach (var book in books)
            {
                book.IsFavorite = await _favoriteRepository.IsBookFavorite(userId, book.Id, cancellationToken);
            }

            return books;
        }
    }
}
