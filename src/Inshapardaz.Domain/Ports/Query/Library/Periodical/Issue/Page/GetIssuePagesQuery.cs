using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.File;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePagesQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int pageNumber,
    int pageSize)
    : LibraryBaseQuery<Page<IssuePageModel>>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int PageNumber { get; private set; } = pageNumber;
    public int PageSize { get; private set; } = pageSize;
    public EditingStatus StatusFilter { get; set; }
    public AssignmentFilter WriterAssignmentFilter { get; set; }
    public int? AccountId { get; set; }
    public AssignmentFilter ReviewerAssignmentFilter { get; set; }
}

public class GetIssuePagesQueryHandler(IIssuePageRepository issuePageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : QueryHandlerAsync<GetIssuePagesQuery, Page<IssuePageModel>>
{

    public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await issuePageRepository.GetPagesByIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, query.PageNumber, query.PageSize, query.StatusFilter, query.WriterAssignmentFilter, query.ReviewerAssignmentFilter, query.AccountId, cancellationToken);

        foreach (var page in pages.Data)
        {
            if (page.FileId.HasValue)
            {
                var file = await fileRepository.GetFileById(page.FileId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
        }

        return pages;
    }
}
