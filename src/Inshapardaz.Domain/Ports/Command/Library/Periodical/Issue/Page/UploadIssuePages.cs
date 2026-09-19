using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UploadIssuePages(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber)
    : LibraryBaseCommand(libraryId)
{
    public IEnumerable<FileModel> Files { get; set; }
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class UploadIssuePagesHandler(
    IIssuePageRepository issuePageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage,
    IConvertPdf pdfConverter,
    IOpenZip zipOpener)
    : RequestHandlerAsync<UploadIssuePages>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UploadIssuePages> HandleAsync(UploadIssuePages command, CancellationToken cancellationToken = new CancellationToken())
    {
        var pageNumber = await issuePageRepository.GetLastPageNumberForIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        IEnumerable<FileModel> files;
        if (command.Files.Count() == 1 && command.Files.Single().MimeType == MimeTypes.Pdf)
        {
            var pages = pdfConverter.ExtractImagePages(command.Files.Single().Contents);
            files = pages.Select(p => new FileModel()
            {
                Contents = p.Value,
                FileName = p.Key,
                MimeType = MimeTypes.Jpg
            });
        }
        else if (command.Files.Count() == 1 && (command.Files.Single().MimeType == MimeTypes.Zip || command.Files.Single().MimeType == MimeTypes.CompressedFile))
        {
            files = zipOpener.ExtractImages(command.Files.Single().Contents);
        }
        else
        {
            files = command.Files;
        }

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).Trim('.');
            var sequenceNumber = ++pageNumber;
            var url = await AddImageToFileStore(command.PeriodicalId, command.VolumeNumber, command.IssueNumber,
                sequenceNumber, $"{sequenceNumber:0000}.{extension}", file.Contents, file.MimeType, cancellationToken);
            var fileModel = await fileRepository.AddFile(new FileModel
            {
                IsPublic = false,
                FilePath = url,
                DateCreated = DateTime.UtcNow,
                FileName = file.FileName,
                MimeType = file.MimeType,
                Checksum = ChecksumHelper.Compute(file.Contents)
            }, cancellationToken);
            var bookPage = await issuePageRepository.AddPage(command.LibraryId, new IssuePageModel()
            {
                PeriodicalId = command.PeriodicalId,
                VolumeNumber = command.VolumeNumber,
                IssueNumber = command.IssueNumber,
                SequenceNumber = pageNumber,
                ImageId = fileModel.Id,
                Status = EditingStatus.Available
            }, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(periodicalId, volumeNumber, issueNumber, sequenceNumber, fileName);
        return await fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string fileName) => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{fileName}";
}
