using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByAuthorQuery : IQuery<Page<BookModel>>
    {
        public GetBooksByAuthorQuery(int authorId, int pageNumber, int pageSize)
        {
            AuthorId = authorId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int AuthorId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; set; }
    }

    public class GetBooksByAuthorQueryHandler : QueryHandlerAsync<GetBooksByAuthorQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBooksByAuthorQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksByAuthorQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksByAuthor(command.AuthorId, command.PageNumber, command.PageSize, cancellationToken);
            return await MarkFavorites(books, command.UserId, cancellationToken);
        }

        private async Task<Page<BookModel>> MarkFavorites(Page<BookModel> books, Guid userId, CancellationToken cancellationToken)
        {
            if (userId != Guid.Empty)
            {
                foreach (var book in books.Data)
                {
                    book.IsFavorite = await _favoriteRepository.IsBookFavorite(userId, book.Id, cancellationToken);
                }
            }

            return books;
        }
    }
}
