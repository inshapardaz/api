using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Series;

public class DeleteSeriesRequest(int libraryId, int seriesId) : LibraryBaseCommand(libraryId)
{
    public int SeriesId { get; } = seriesId;
}

public class DeleteSeriesRequestHandler(ISeriesRepository seriesRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteSeriesRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteSeriesRequest> HandleAsync(DeleteSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = await seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);
        if (series != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(series.ImageId), cancellationToken: cancellationToken);
            await seriesRepository.DeleteSeries(command.LibraryId, command.SeriesId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
