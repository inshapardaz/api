using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Query.File;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePageByNumberQuery : LibraryBaseQuery<IssuePageModel>
{
    public GetIssuePageByNumberQuery(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int sequenceNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }
}

public class GetIssuePageByNumberQueryHandler : QueryHandlerAsync<GetIssuePageByNumberQuery, IssuePageModel>
{
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IQueryProcessor _queryProcessor;

    public GetIssuePageByNumberQueryHandler(IIssuePageRepository issuePageRepository, IQueryProcessor queryProcessor)
    {
        _issuePageRepository = issuePageRepository;
        _queryProcessor = queryProcessor;
    }

    public override async Task<IssuePageModel> ExecuteAsync(GetIssuePageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (page != null)
        {
            if (page.FileId.HasValue)
            {
                page.Text = await _queryProcessor.ExecuteAsync(new GetTextFileQuery(page.FileId.Value), cancellationToken);
            }

            var previousPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
            var nextPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);

            page.PreviousPage = previousPage;
            page.NextPage = nextPage;
        }

        return page;
    }
}
