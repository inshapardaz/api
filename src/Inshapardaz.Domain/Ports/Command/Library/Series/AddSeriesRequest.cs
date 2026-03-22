using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Series;

public class AddSeriesRequest(int libraryId, SeriesModel series) : LibraryBaseCommand(libraryId)
{
    public SeriesModel Series { get; } = series;
    public SeriesModel Result { get; set; }
}

public class AddSeriesRequestHandler(ISeriesRepository seriesRepository) : RequestHandlerAsync<AddSeriesRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<AddSeriesRequest> HandleAsync(AddSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await seriesRepository.AddSeries(command.LibraryId, command.Series, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
