using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class UpdateIssueContentRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    long contentId,
    string language,
    string mimeType)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public long ContentId { get; } = contentId;
    public string Language { get; } = language;
    public string MimeType { get; } = mimeType;

    public FileModel Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssueContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssueContentRequestHandler(
    IIssueRepository issueRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateIssueContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueContentRequest> HandleAsync(UpdateIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue != null)
        {
            var issueContent = await issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);

            var fileName = FilePathHelper.GetIssueContentFileName(command.Content.FileName);
            var filePath = FilePathHelper.GetIssueContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName);

            var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Content.Contents)
            {
                MimeType = command.MimeType,
                ExistingFileId = issueContent?.FileId,
                FileName = command.Content.FileName
            };

            await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);

            if (issueContent == null)
            {
                command.Result.Content = await issueRepository.AddIssueContent(command.LibraryId,
                    new IssueContentModel
                    {
                        PeriodicalId = issue.PeriodicalId,
                        VolumeNumber = issue.VolumeNumber,
                        IssueNumber = issue.IssueNumber,
                        FileId = saveContentCommand.Result.Id,
                        FileName = saveContentCommand.Result.FileName,
                        Language = command.Language,
                        MimeType = command.MimeType,
                    }, 
                    cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                issueContent.Language = command.Language;
                issueContent.MimeType = command.MimeType;
                issueContent.FileId = saveContentCommand.Result.Id;
                issueContent.FileName = saveContentCommand.Result.FileName;
                command.Result.Content = await issueRepository.UpdateIssueContent(command.LibraryId, issueContent, cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
