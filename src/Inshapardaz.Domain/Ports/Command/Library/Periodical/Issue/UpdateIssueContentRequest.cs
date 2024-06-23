using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class UpdateIssueContentRequest : LibraryBaseCommand
{
    public UpdateIssueContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, string language, string mimeType)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        ContentId = contentId;
        Language = language;
        MimeType = mimeType;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public long ContentId { get; }
    public string Language { get; }
    public string MimeType { get; }
    
    public FileModel Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssueContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssueContentRequestHandler : RequestHandlerAsync<UpdateIssueContentRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IAmACommandProcessor _commandProcessor;


    public UpdateIssueContentRequestHandler(IIssueRepository issueRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueContentRequest> HandleAsync(UpdateIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue != null)
        {
            var issueContent = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);

            var fileName = FilePathHelper.GetIssueContentFileName(command.Content.FileName);
            var filePath = FilePathHelper.GetIssueContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName);

            var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Content.Contents)
            {
                MimeType = command.MimeType,
                ExistingFileId = issueContent?.FileId
            };

            await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);

            if (issueContent == null)
            {
                command.Result.Content = await _issueRepository.AddIssueContent(command.LibraryId,
                    new IssueContentModel
                    {
                        PeriodicalId = issue.PeriodicalId,
                        VolumeNumber = issue.VolumeNumber,
                        IssueNumber = issue.IssueNumber,
                        FileId = saveContentCommand.Result.Id,
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
                command.Result.Content = await _issueRepository.UpdateIssueContent(command.LibraryId, issueContent, cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
