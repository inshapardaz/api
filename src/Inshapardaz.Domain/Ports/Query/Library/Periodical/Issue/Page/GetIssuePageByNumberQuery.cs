using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using Inshapardaz.Domain.Ports.Query.File;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePageByNumberQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber)
    : LibraryBaseQuery<IssuePageModel>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
}

public class GetIssuePageByNumberQueryHandler(IIssuePageRepository issuePageRepository, IQueryProcessor queryProcessor)
    : QueryHandlerAsync<GetIssuePageByNumberQuery, IssuePageModel>
{
    public override async Task<IssuePageModel> ExecuteAsync(GetIssuePageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (page != null)
        {
            if (page.FileId.HasValue)
            {
                page.Text = await queryProcessor.ExecuteAsync(new GetTextFileQuery(page.FileId.Value), cancellationToken);
            }

            var previousPage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
            var nextPage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);

            page.PreviousPage = previousPage;
            page.NextPage = nextPage;
        }

        return page;
    }
}
