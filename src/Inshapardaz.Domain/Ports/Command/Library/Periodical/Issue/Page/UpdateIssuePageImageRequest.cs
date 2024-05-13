using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageImageRequest : LibraryBaseCommand
{
    public UpdateIssuePageImageRequest(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int sequenceNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssuePageImageRequestHandler : RequestHandlerAsync<UpdateIssuePageImageRequest>
{
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public UpdateIssuePageImageRequestHandler(IIssuePageRepository issuePageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _issuePageRepository = issuePageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageImageRequest> HandleAsync(UpdateIssuePageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (issuePage == null)
        {
            throw new NotFoundException();
        }

        if (issuePage.ImageId.HasValue)
        {
            command.Image.Id = issuePage.ImageId.Value;
            var existingImage = await _fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);

            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            await _fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = issuePage.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await _issuePageRepository.UpdatePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int periodicalId, int volumeNumber,
                            int issueNumber, int sequenceNumber, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(periodicalId, volumeNumber, issueNumber, sequenceNumber, fileName);
        return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int periodicalId, int volumeNumber,
                            int issueNumber, int sequenceNumber, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/page_{sequenceNumber:0000}.{fileNameWithourExtension}";
    }
}
