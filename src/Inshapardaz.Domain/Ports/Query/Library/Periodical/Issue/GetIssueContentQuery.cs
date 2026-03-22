using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssueContentQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int contentId,
    int? accountId)
    : LibraryBaseQuery<IssueContentModel>(libraryId)
{
    public int PeriodicalId { get; set; } = periodicalId;
    public int VolumeNumber { get; set; } = volumeNumber;
    public int IssueNumber { get; set; } = issueNumber;

    public int ContentId { get; set; } = contentId;
    public int? AccountId { get; } = accountId;
}

public class GetIssueContentQueryHandler(
    ILibraryRepository libraryRepository,
    IIssueRepository issueRepository,
    IFileRepository fileRepository)
    : QueryHandlerAsync<GetIssueContentQuery, IssueContentModel>
{
    private readonly ILibraryRepository _libraryRepository = libraryRepository;
    private readonly IFileRepository _fileRepository = fileRepository;

    public override async Task<IssueContentModel> ExecuteAsync(GetIssueContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new NotFoundException();
        }

        if (!issue.IsPublic && !command.AccountId.HasValue)
        {
            throw new UnauthorizedException();
        }

        var issueContent = await issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);

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
