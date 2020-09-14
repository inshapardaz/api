using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteSeriesRequest : LibraryAuthorisedCommand
    {
        public DeleteSeriesRequest(ClaimsPrincipal claims, int libraryId, int seriesId)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<DeleteSeriesRequest> HandleAsync(DeleteSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _seriesRepository.DeleteSeries(command.LibraryId, command.SeriesId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
