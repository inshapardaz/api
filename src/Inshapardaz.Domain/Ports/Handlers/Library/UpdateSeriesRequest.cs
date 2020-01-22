using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateSeriesRequest : RequestBase
    {
        public UpdateSeriesRequest(SeriesModel series)
        {
            Series = series;
        }

        public SeriesModel Series { get; }

        public UpdateSeriesResult Result { get; } = new UpdateSeriesResult();

        public class UpdateSeriesResult
        {
            public bool HasAddedNew { get; set; }

            public SeriesModel Series { get; set; }
        }
    }

    public class UpdateSeriesRequestHandler : RequestHandlerAsync<UpdateSeriesRequest>
    {
        private readonly ISeriesRepository _seriesRepository;

        public UpdateSeriesRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<UpdateSeriesRequest> HandleAsync(UpdateSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _seriesRepository.GetSeriesById(command.Series.Id, cancellationToken);

            if (result == null)
            {
                command.Series.Id = default(int);
                var newSeries = await _seriesRepository.AddSeries(command.Series, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Series = newSeries;
            }
            else
            {
                await _seriesRepository.UpdateSeries(command.Series, cancellationToken);
                command.Result.Series = command.Series;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}