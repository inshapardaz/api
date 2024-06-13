using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteSeriesRequestHandler(ISeriesRepository seriesRepository, IAmACommandProcessor commandProcessor)
    {
        _seriesRepository = seriesRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteSeriesRequest> HandleAsync(DeleteSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = await _seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);
        if (series != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(series.ImageId), cancellationToken: cancellationToken);
            await _seriesRepository.DeleteSeries(command.LibraryId, command.SeriesId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
