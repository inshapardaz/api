using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteSeriesRequest : RequestBase
    {
        public DeleteSeriesRequest(int seriesId)
        {
            SeriesId = seriesId;
        }

        public int SeriesId { get; }
    }

    public class DeleteSeriesRequestHandler : RequestHandlerAsync<DeleteSeriesRequest>
    {
        private readonly ISeriesRepository _seriesRepository;

        public DeleteSeriesRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<DeleteSeriesRequest> HandleAsync(DeleteSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _seriesRepository.DeleteSeries(command.SeriesId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
