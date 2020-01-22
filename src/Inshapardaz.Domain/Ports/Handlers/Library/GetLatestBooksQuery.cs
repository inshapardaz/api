using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetLatestBooksQuery : IQuery<IEnumerable<BookModel>>
    {
        public Guid UserId { get; }

        public GetLatestBooksQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetLatestBooksQueryHandler : QueryHandlerAsync<GetLatestBooksQuery, IEnumerable<BookModel>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetLatestBooksQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<IEnumerable<BookModel>> ExecuteAsync(GetLatestBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetLatestBooks(cancellationToken);
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
