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

        public GetSeriesRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<GetSeriesRequest> HandleAsync(GetSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _seriesRepository.GetSeries(cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
