using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssuesYearQuery(int libraryId, int periodicalId)
    : LibraryBaseQuery<IEnumerable<(int Year, int count)>>(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public SortDirection SortDirection { get; set; }
    public AssignmentStatus AssignmentStatus { get; set; }
}

public class GetIssuesYearQueryHandler(
    IIssueRepository issueRepository,
    IPeriodicalRepository periodicalRepository,
    IFileRepository fileRepository)
    : QueryHandlerAsync<GetIssuesYearQuery, IEnumerable<(int Year, int count)>>
{
    private readonly IFileRepository _fileRepository = fileRepository;

    public override async Task<IEnumerable<(int Year, int count)>> ExecuteAsync(GetIssuesYearQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        if (periodical == null) return null;
        var issues = await issueRepository.GetIssuesYear(command.LibraryId, command.PeriodicalId, command.AssignmentStatus, command.SortDirection, cancellationToken);

        return issues;
    }
}
