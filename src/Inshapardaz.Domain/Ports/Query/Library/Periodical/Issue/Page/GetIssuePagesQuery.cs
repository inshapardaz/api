using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePagesQuery : LibraryBaseQuery<Page<IssuePageModel>>
{
    public GetIssuePagesQuery(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int pageNumber,
                            int pageSize)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public EditingStatus StatusFilter { get; set; }
    public AssignmentFilter AssignmentFilter { get; set; }
    public int? AccountId { get; set; }
    public AssignmentFilter ReviewerAssignmentFilter { get; set; }
}

public class GetIssuePagesQueryHandler : QueryHandlerAsync<GetIssuePagesQuery, Page<IssuePageModel>>
{
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetIssuePagesQueryHandler(IIssuePageRepository issuePageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _issuePageRepository = issuePageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await _issuePageRepository.GetPagesByIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, query.PageNumber, query.PageSize, query.StatusFilter, query.AssignmentFilter, query.ReviewerAssignmentFilter, query.AccountId, cancellationToken);

        foreach (var page in pages.Data)
        {
            if (page.FileId.HasValue)
            {
                var file = await _fileRepository.GetFileById(page.FileId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
        }

        return pages;
    }
}
