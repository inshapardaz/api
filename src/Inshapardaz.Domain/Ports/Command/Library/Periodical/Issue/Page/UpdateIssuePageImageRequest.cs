using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageImageRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssuePageImageRequestHandler(
    IIssuePageRepository issuePageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<UpdateIssuePageImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageImageRequest> HandleAsync(UpdateIssuePageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (issuePage == null)
        {
            throw new NotFoundException();
        }

        command.Image.Checksum = ChecksumHelper.Compute(command.Image.Contents);

        if (issuePage.ImageId.HasValue)
        {
            command.Image.Id = issuePage.ImageId.Value;
            var existingImage = await fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);

            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            await fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = issuePage.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            command.Result.File = await fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await issuePageRepository.UpdatePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int periodicalId, int volumeNumber,
                            int issueNumber, int sequenceNumber, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(periodicalId, volumeNumber, issueNumber, sequenceNumber, fileName);
        return await fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int periodicalId, int volumeNumber,
                            int issueNumber, int sequenceNumber, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/page_{sequenceNumber:0000}.{fileNameWithourExtension}";
    }
}
