using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageRequest : LibraryBaseCommand
{
    public UpdateIssuePageRequest(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int sequenceNumber,
                            int? accountId,
                            IssuePageModel page)
    : base(libraryId)
    {
        IssuePage = page;
        IssuePage.PeriodicalId = periodicalId;
        IssuePage.VolumeNumber = volumeNumber;
        IssuePage.IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        AccountId = accountId;
    }

    public IssuePageModel IssuePage { get; }

    public RequestResult Result { get; set; } = new RequestResult();
    public int SequenceNumber { get; set; }
    public int? AccountId { get; }

    public class RequestResult
    {
        public IssuePageModel IssuePage { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssuePageRequestHandler : RequestHandlerAsync<UpdateIssuePageRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssuePageRepository _issuePageRepository;

    public UpdateIssuePageRequestHandler(IIssueRepository issueRepository,
                                     IIssuePageRepository issuePageRepository)
    {
        _issueRepository = issueRepository;
        _issuePageRepository = issuePageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageRequest> HandleAsync(UpdateIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var existingIssuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, cancellationToken);

        if (existingIssuePage == null)
        {
            command.Result.IssuePage = await _issuePageRepository.AddPage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.Text, 0, command.IssuePage.ArticleId, command.IssuePage.Status, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.IssuePage = await _issuePageRepository.UpdatePage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.Text, existingIssuePage.ImageId ?? 0, command.IssuePage.ArticleId, command.IssuePage.Status, cancellationToken);
        }

        var previousPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.SequenceNumber - 1, cancellationToken);
        var nextPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.SequenceNumber + 1, cancellationToken);

        command.Result.IssuePage.PreviousPage = previousPage;
        command.Result.IssuePage.NextPage = nextPage;

        return await base.HandleAsync(command, cancellationToken);
    }
}
