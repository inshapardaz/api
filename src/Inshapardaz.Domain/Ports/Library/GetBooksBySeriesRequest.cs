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

        public GetBooksBySeriesRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetBooksBySeriesRequest> HandleAsync(GetBooksBySeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksBySeries(command.SeriesId, command.PageNumber, command.PageSize, cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
