using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Series
{
    public class UpdateSeriesRequest : LibraryBaseCommand
    {
        public UpdateSeriesRequest(int libraryId, SeriesModel series)
            : base(libraryId)
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
            var result = await _seriesRepository.GetSeriesById(command.LibraryId, command.Series.Id, cancellationToken);

            if (result == null)
            {
                command.Series.Id = default;
                var newSeries = await _seriesRepository.AddSeries(command.LibraryId, command.Series, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Series = newSeries;
            }
            else
            {
                result.Name = command.Series.Name;
                result.Description = command.Series.Description;
                await _seriesRepository.UpdateSeries(command.LibraryId, result, cancellationToken);
                command.Result.Series = command.Series;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
