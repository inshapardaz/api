using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByCategoryQuery : IQuery<Page<Book>>
    {
        public GetBooksByCategoryQuery(int categoryId, int pageNumber, int pageSize)
        {
            CategoryId = categoryId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int CategoryId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; set; }
    }

    public class GetBooksByCategoryQueryHandler : QueryHandlerAsync<GetBooksByCategoryQuery, Page<Book>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBooksByCategoryQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<Page<Book>> ExecuteAsync(GetBooksByCategoryQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksByCategory(command.CategoryId, command.PageNumber, command.PageSize, cancellationToken);
            return await MarkFavorites(books, command.UserId, cancellationToken);
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
