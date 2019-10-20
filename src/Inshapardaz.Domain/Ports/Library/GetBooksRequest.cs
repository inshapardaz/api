using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksRequest : RequestBase
    {
        public GetBooksRequest(int pageNumber, int pageSize, Guid userId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            UserId = userId;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; }

        public Page<Book> Result { get; set; }

        public string Query { get; set; }
    }

    public class GetBooksRequestHandler : RequestHandlerAsync<GetBooksRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBooksRequestHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetBooksRequest> HandleAsync(GetBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                var books = await _bookRepository.GetBooks(command.PageNumber, command.PageSize, cancellationToken);
                command.Result = await MarkFavorites(books, command.UserId, cancellationToken);
            }
            else
            {
                var books = await _bookRepository.SearchBooks(command.Query, command.PageNumber, command.PageSize, cancellationToken);
                command.Result = await MarkFavorites(books, command.UserId, cancellationToken);
                command.Result = books;
            }

            return await base.HandleAsync(command, cancellationToken);
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
