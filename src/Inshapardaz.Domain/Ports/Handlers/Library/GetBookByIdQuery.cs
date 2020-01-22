using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBookByIdQuery : IQuery<BookModel>
    {
        public GetBookByIdQuery(int bookId, Guid userId)
        {
            BookId = bookId;
            UserId = userId;
        }

        public int BookId { get; private set; }

        public Guid UserId { get; private set; }
    }

    public class GetBookByIdQueryHandler : QueryHandlerAsync<GetBookByIdQuery, BookModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<BookModel> ExecuteAsync(GetBookByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);
            if (book != null && command.UserId != Guid.Empty)
            {
                book.IsFavorite = await _favoriteRepository.IsBookFavorite(command.UserId, command.BookId, cancellationToken);
            }

            return book;
        }
    }
}

