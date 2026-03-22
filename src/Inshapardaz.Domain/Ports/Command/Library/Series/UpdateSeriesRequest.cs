using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Series;

public class UpdateSeriesRequest(int libraryId, SeriesModel series) : LibraryBaseCommand(libraryId)
{
    public SeriesModel Series { get; } = series;

    public UpdateSeriesResult Result { get; } = new UpdateSeriesResult();

    public class UpdateSeriesResult
    {
        public bool HasAddedNew { get; set; }

        public SeriesModel Series { get; set; }
    }
}

public class UpdateSeriesRequestHandler(ISeriesRepository seriesRepository) : RequestHandlerAsync<UpdateSeriesRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateSeriesRequest> HandleAsync(UpdateSeriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await seriesRepository.GetSeriesById(command.LibraryId, command.Series.Id, cancellationToken);

        if (result == null)
        {
            command.Series.Id = default;
            var newSeries = await seriesRepository.AddSeries(command.LibraryId, command.Series, cancellationToken);
            command.Result.HasAddedNew = true;
            command.Result.Series = newSeries;
        }
        else
        {
            result.Name = command.Series.Name;
            result.Description = command.Series.Description;
            await seriesRepository.UpdateSeries(command.LibraryId, result, cancellationToken);
            command.Result.Series = command.Series;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
