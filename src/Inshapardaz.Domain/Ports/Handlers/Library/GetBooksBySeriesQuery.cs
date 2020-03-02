using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksBySeriesQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetBooksBySeriesQuery(int libraryId, int seriesId, int pageNumber, int pageSize, Guid userId)
            : base(libraryId, userId)
        {
            SeriesId = seriesId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int SeriesId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetBooksBySeriesQueryHandler : QueryHandlerAsync<GetBooksBySeriesQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksBySeriesQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksBySeriesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetBooksBySeries(command.LibraryId, command.SeriesId, command.PageNumber, command.PageSize, command.UserId, cancellationToken);
        }
    }
}
