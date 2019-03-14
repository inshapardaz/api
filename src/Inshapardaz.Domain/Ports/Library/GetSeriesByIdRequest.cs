using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesByIdRequest : RequestBase
    {
        public GetSeriesByIdRequest(int seriesId)
        {
            SeriesId = seriesId;
        }

        public Series Result { get; set; }
        public int SeriesId { get; }
    }

    public class GetSeriesByIdRequestHandler : RequestHandlerAsync<GetSeriesByIdRequest>
    {
        private readonly ISeriesRepository _seriesRepository;

        public GetSeriesByIdRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<GetSeriesByIdRequest> HandleAsync(GetSeriesByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var series = await _seriesRepository.GetSeriesById(command.SeriesId, cancellationToken);
            command.Result = series;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

