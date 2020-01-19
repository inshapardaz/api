using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesByIdQuery : IQuery<Series>
    {
        public GetSeriesByIdQuery(int seriesId)
        {
            SeriesId = seriesId;
        }

        public int SeriesId { get; }
    }

    public class GetSeriesByIdQueryHandler : QueryHandlerAsync<GetSeriesByIdQuery, Series>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IBookRepository _bookRepository;

        public GetSeriesByIdQueryHandler(ISeriesRepository seriesRepository, IBookRepository bookRepository)
        {
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<Series> ExecuteAsync(GetSeriesByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var series = await _seriesRepository.GetSeriesById(command.SeriesId, cancellationToken);

            if (series != null)
            {
                series.BookCount = await _bookRepository.GetBookCountBySeries(command.SeriesId, cancellationToken);
            }

            return series;
        }
    }
}

