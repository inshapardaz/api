using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddSeriesRequest : LibraryAuthorisedCommand
    {
        public AddSeriesRequest(ClaimsPrincipal claims, int libraryId, SeriesModel series)
            : base(claims, libraryId)
        {
            Series = series;
        }

        public SeriesModel Series { get; }
        public SeriesModel Result { get; set; }
    }

    public class AddSeriesRequestHandler : RequestHandlerAsync<AddSeriesRequest>
    {
        private readonly ISeriesRepository _seriesRepository;

        public AddSeriesRequestHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddSeriesRequest> HandleAsync(AddSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _seriesRepository.AddSeries(command.LibraryId, command.Series, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
