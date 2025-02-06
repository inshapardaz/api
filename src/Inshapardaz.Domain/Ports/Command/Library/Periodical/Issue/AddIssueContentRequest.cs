using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class AddIssueContentRequest : LibraryBaseCommand
{
    public AddIssueContentRequest(int libraryId, int periodicalId, int VolumeNumber, int issueNumber, string language, string mimeType)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        this.VolumeNumber = VolumeNumber;
        IssueNumber = issueNumber;
        Language = language;
        MimeType = mimeType;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }

    public string Language { get; }
    public string MimeType { get; }
    public FileModel Content { get; set; }

    public IssueContentModel Result { get; set; }
}

public class AddIssueContentRequestHandler : RequestHandlerAsync<AddIssueContentRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public AddIssueContentRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssueContentRequest> HandleAsync(AddIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue != null)
        {
            var fileName = FilePathHelper.GetIssueContentFileName(command.Content.FileName);

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, FilePathHelper.GetIssueContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName), command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                IsPublic = command.Content.IsPublic
            };

            await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            command.Result = await _issueRepository.AddIssueContent(command.LibraryId,
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
