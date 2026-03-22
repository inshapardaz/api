using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Series;

public class UpdateSeriesImageRequest(int libraryId, int seriesId) : LibraryBaseCommand(libraryId)
{
    public int SeriesId { get; } = seriesId;

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateSeriesImageRequestHandler(ISeriesRepository seriesRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateSeriesImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateSeriesImageRequest> HandleAsync(UpdateSeriesImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = await seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);

        if (series == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetSeriesImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetSeriesImagePath(command.SeriesId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = series.ImageId,
            IsPublic = true,
        };

        await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.Result.File = saveContentCommand.Result;
        if (!series.ImageId.HasValue)
        {
            command.Result.HasAddedNew = true;
            await seriesRepository.UpdateSeriesImage(command.LibraryId, command.SeriesId, command.Result.File.Id, cancellationToken);
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
