using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksBySeriesRequest : RequestBase
    {
        public GetBooksBySeriesRequest(int seriesId, int pageNumber, int pageSize)
        {
            SeriesId = seriesId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int SeriesId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; set; }

        public Page<Book> Result { get; set; }
    }

    public class GetBooksBySeriesRequestHandler : RequestHandlerAsync<GetBooksBySeriesRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBooksBySeriesRequestHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetBooksBySeriesRequest> HandleAsync(GetBooksBySeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksBySeries(command.SeriesId, command.PageNumber, command.PageSize, cancellationToken);
            command.Result = await MarkFavorites(books, command.UserId, cancellationToken);
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
