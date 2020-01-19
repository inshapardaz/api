using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksQuery : IQuery<Page<Book>>
    {
        public GetBooksQuery(int pageNumber, int pageSize, Guid userId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            UserId = userId;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; }

        public string Query { get; set; }
    }

    public class GetBooksQueryHandler : QueryHandlerAsync<GetBooksQuery, Page<Book>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBooksQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<Page<Book>> ExecuteAsync(GetBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                var books = await _bookRepository.GetBooks(command.PageNumber, command.PageSize, cancellationToken);
                return await MarkFavorites(books, command.UserId, cancellationToken);
            }
            else
            {
                var books = await _bookRepository.SearchBooks(command.Query, command.PageNumber, command.PageSize, cancellationToken);
                return await MarkFavorites(books, command.UserId, cancellationToken);
            }
        }

        private async Task<Page<Book>> MarkFavorites(Page<Book> books, Guid userId, CancellationToken cancellationToken)
        {
            foreach (var book in books.Data)
            {
                book.IsFavorite = await _favoriteRepository.IsBookFavorite(userId, book.Id, cancellationToken);
            }

            return books;
        }
    }
}
