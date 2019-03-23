using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesRequest : RequestBase
    {
        public IEnumerable<Series> Result { get; set; }
    }

    public class GetSeriesRequestHandler : RequestHandlerAsync<GetSeriesRequest>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IBookRepository _bookRepository;

        public GetSeriesRequestHandler(ISeriesRepository seriesRepository, IBookRepository bookRepository)
        {
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<GetSeriesRequest> HandleAsync(GetSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _seriesRepository.GetSeries(cancellationToken);

            foreach (var series in command.Result)
            {
                series.BookCount = await _bookRepository.GetBookCountBySeries(series.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
