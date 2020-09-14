using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateSeriesRequest : LibraryAuthorisedCommand
    {
        public UpdateSeriesRequest(ClaimsPrincipal claims, int libraryId, SeriesModel series)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<UpdateSeriesRequest> HandleAsync(UpdateSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _seriesRepository.GetSeriesById(command.LibraryId, command.Series.Id, cancellationToken);

            if (result == null)
            {
                command.Series.Id = default(int);
                var newSeries = await _seriesRepository.AddSeries(command.LibraryId, command.Series, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Series = newSeries;
            }
            else
            {
                await _seriesRepository.UpdateSeries(command.LibraryId, command.Series, cancellationToken);
                command.Result.Series = command.Series;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
