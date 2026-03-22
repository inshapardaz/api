using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class UpdateIssueImageRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssueImageRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateIssueImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueImageRequest> HandleAsync(UpdateIssueImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetPeriodicalIssueImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetPeriodicalIssueImageFilePath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = issue.ImageId,
            IsPublic = true
        };

        await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);


        command.Result.File = saveContentCommand.Result;

        if (!issue.ImageId.HasValue)
        {
            issue.ImageId = saveContentCommand.Result.Id;
            await issueRepository.UpdateIssue(command.LibraryId, command.PeriodicalId, issue, cancellationToken);
            command.Result.HasAddedNew = true;

        }

        return await base.HandleAsync(command, cancellationToken);
    }

}
