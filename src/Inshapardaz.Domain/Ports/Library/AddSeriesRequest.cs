using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddSeriesRequest : RequestBase
    {
        public AddSeriesRequest(Series series)
        {
            Series = series;
        }

        public Series Series { get; }
        public Series Result { get; set; }
    }

    public class AddSeriesRequestHandler : RequestHandlerAsync<AddSeriesRequest>
    {
        private readonly ISeriesRepository _seriesRepository;

        public AddSeriesRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<AddSeriesRequest> HandleAsync(AddSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _seriesRepository.AddSeries(command.Series, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
