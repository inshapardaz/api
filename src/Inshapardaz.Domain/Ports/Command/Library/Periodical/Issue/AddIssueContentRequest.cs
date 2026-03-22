using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class AddIssueContentRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    string language,
    string mimeType)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;

    public string Language { get; } = language;
    public string MimeType { get; } = mimeType;
    public FileModel Content { get; set; }

    public IssueContentModel Result { get; set; }
}

public class AddIssueContentRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<AddIssueContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssueContentRequest> HandleAsync(AddIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue != null)
        {
            var fileName = FilePathHelper.GetIssueContentFileName(command.Content.FileName);

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, FilePathHelper.GetIssueContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName), command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                IsPublic = command.Content.IsPublic
            };

            await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            command.Result = await issueRepository.AddIssueContent(command.LibraryId,
                new IssueContentModel
                {
                    PeriodicalId = issue.PeriodicalId,
                    VolumeNumber = issue.VolumeNumber,
                    IssueNumber = issue.IssueNumber,
                    FileId = saveFileCommand.Result.Id,
                    FileName = saveFileCommand.Result.FileName,
                    Language = command.Language,
                    MimeType = command.MimeType,
                },
                cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
