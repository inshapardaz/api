using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class UpdateIssueRequest : LibraryBaseCommand
{
    public UpdateIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueModel issue)
        : base(libraryId)
    {
        Issue = issue;
        Issue.PeriodicalId = periodicalId;
        Issue.VolumeNumber = volumeNumber;
        Issue.IssueNumber = issueNumber;
    }

    public IssueModel Issue { get; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssueModel Issue { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssueRequestHandler : RequestHandlerAsync<UpdateIssueRequest>
{
    private readonly IIssueRepository _issueRepository;

    public UpdateIssueRequestHandler(IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueRequest> HandleAsync(UpdateIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _issueRepository.GetIssue(command.LibraryId, command.Issue.PeriodicalId, command.Issue.VolumeNumber, command.Issue.IssueNumber, cancellationToken);

        if (result == null)
        {
            var Issue = command.Issue;
            Issue.Id = default;
            command.Result.Issue = await _issueRepository.AddIssue(command.LibraryId, command.Issue.PeriodicalId, Issue, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            result.IssueNumber = command.Issue.IssueNumber;
            result.VolumeNumber = command.Issue.VolumeNumber;
            result.IssueDate = command.Issue.IssueDate;
            result.Status = command.Issue.Status;
            result.IsPublic = command.Issue.IsPublic;
            result.ImageId = command.Issue.ImageId;
            result.Tags = command.Issue.Tags;

            await _issueRepository.UpdateIssue(command.LibraryId, command.Issue.PeriodicalId, result, cancellationToken);
            command.Result.Issue = result;
        }
        command.Issue.PeriodicalId = command.Issue.PeriodicalId;

        return await base.HandleAsync(command, cancellationToken);
    }
}
