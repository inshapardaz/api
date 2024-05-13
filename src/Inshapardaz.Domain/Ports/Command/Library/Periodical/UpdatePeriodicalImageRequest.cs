using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class UpdatePeriodicalImageRequest : LibraryBaseCommand
{
    public UpdatePeriodicalImageRequest(int libraryId, int periodicalId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
    }

    public int PeriodicalId { get; }

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdatePeriodicalImageRequestHandler : RequestHandlerAsync<UpdatePeriodicalImageRequest>
{
    private readonly IPeriodicalRepository _periodicalRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public UpdatePeriodicalImageRequestHandler(IPeriodicalRepository PeriodicalRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _periodicalRepository = PeriodicalRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<UpdatePeriodicalImageRequest> HandleAsync(UpdatePeriodicalImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);

        if (periodical == null)
        {
            throw new NotFoundException();
        }

        if (periodical.ImageId.HasValue)
        {
            command.Image.Id = periodical.ImageId.Value;
            var existingImage = await _fileRepository.GetFileById(periodical.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(periodical.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            await _fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = periodical.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(periodical.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await _periodicalRepository.UpdatePeriodicalImage(command.LibraryId, periodical.Id, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int PeriodicalId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(PeriodicalId, fileName);
        return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int PeriodicalId, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"periodicals/{PeriodicalId}/title.{fileNameWithourExtension}";
    }
}
