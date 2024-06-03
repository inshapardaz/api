using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssueContentQuery : LibraryBaseQuery<IssueContentModel>
{
    public GetIssueContentQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int contentId, int? accountId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        AccountId = accountId;
        ContentId = contentId;
    }

    public int PeriodicalId { get; set; }
    public int VolumeNumber { get; set; }
    public int IssueNumber { get; set; }

    public int ContentId { get; set; }
    public int? AccountId { get; }
}

public class GetIssueContentQueryHandler : QueryHandlerAsync<GetIssueContentQuery, IssueContentModel>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IFileRepository _fileRepository;

    public GetIssueContentQueryHandler(ILibraryRepository libraryRepository, IIssueRepository issueRepository, IFileRepository fileRepository)
    {
        _libraryRepository = libraryRepository;
        _issueRepository = issueRepository;
        _fileRepository = fileRepository;
    }

    public override async Task<IssueContentModel> ExecuteAsync(GetIssueContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new NotFoundException();
        }

        if (!issue.IsPublic && !command.AccountId.HasValue)
        {
            throw new UnauthorizedException();
        }

        var issueContent = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);

        //if (bookContent != null)
        //{
        //    if (command.AccountId.HasValue)
        //    {
        //        await _issueRepository.AddRecentBook(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);
        //    }

        //    if (issue.IsPublic)
        //    {
        //        bookContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(bookContent.FileId, _fileRepository, cancellationToken);
        //    }
        //}

        return issueContent;
    }
}
