using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

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
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetIssuePageByNumberQueryHandler(IIssuePageRepository issuePageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _issuePageRepository = issuePageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<IssuePageModel> ExecuteAsync(GetIssuePageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (page != null)
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

            var previousPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
            var nextPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);

            page.PreviousPage = previousPage;
            page.NextPage = nextPage;
        }

        return page;
    }
}
