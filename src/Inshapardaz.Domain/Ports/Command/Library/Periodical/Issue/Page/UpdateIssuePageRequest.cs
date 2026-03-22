using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageRequest(int libraryId, IssuePageModel model) : LibraryBaseCommand(libraryId)
{
    public IssuePageModel IssuePage { get; set; } = model;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssuePageModel IssuePage { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssuePageRequestHandler(
    IIssueRepository issueRepository,
    IIssuePageRepository issuePageRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateIssuePageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageRequest> HandleAsync(UpdateIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var existingIssuePage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, cancellationToken);

        var fileName = FilePathHelper.IssuePageContentFileName;
        var filePath = FilePathHelper.GetIssuePageContentPath(command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, fileName);

        var saveContentCommand = new SaveTextFileCommand(fileName, filePath, command.IssuePage.Text)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = existingIssuePage?.FileId
        };

        await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.IssuePage.FileId = saveContentCommand.Result.Id;
        command.IssuePage.ImageId = existingIssuePage?.ImageId;
        
        if (existingIssuePage == null)
        {
            command.Result.IssuePage = await issuePageRepository.AddPage(command.LibraryId, command.IssuePage, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.IssuePage = await issuePageRepository.UpdatePage(command.LibraryId, command.IssuePage, cancellationToken);
        }

        command.Result.IssuePage.FileId = saveContentCommand.Result.Id;
        command.Result.IssuePage.Text = command.IssuePage.Text;

        var previousPage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber - 1, cancellationToken);
        var nextPage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber + 1, cancellationToken);

        command.Result.IssuePage.PreviousPage = previousPage;
        command.Result.IssuePage.NextPage = nextPage;

        return await base.HandleAsync(command, cancellationToken);
    }
}
