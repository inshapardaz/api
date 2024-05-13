using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Series;

public class DeleteSeriesRequest : LibraryBaseCommand
{
    public DeleteSeriesRequest(int libraryId, int seriesId)
        : base(libraryId)
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

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteSeriesRequest> HandleAsync(DeleteSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await _seriesRepository.DeleteSeries(command.LibraryId, command.SeriesId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
