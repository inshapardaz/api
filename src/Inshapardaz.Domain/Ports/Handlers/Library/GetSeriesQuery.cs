using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesQuery : IQuery<IEnumerable<SeriesModel>>
    {
    }

    public class GetSeriesQueryHandler : QueryHandlerAsync<GetSeriesQuery, IEnumerable<SeriesModel>>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IBookRepository _bookRepository;

        public GetSeriesQueryHandler(ISeriesRepository seriesRepository, IBookRepository bookRepository)
        {
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<IEnumerable<SeriesModel>> ExecuteAsync(GetSeriesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var seriesList = await _seriesRepository.GetSeries(cancellationToken);

            foreach (var series in seriesList)
            {
                series.BookCount = await _bookRepository.GetBookCountBySeries(series.Id, cancellationToken);
            }

            return seriesList;
        }
    }
}
