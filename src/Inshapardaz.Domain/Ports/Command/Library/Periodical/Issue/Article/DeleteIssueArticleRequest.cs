using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class DeleteIssueArticleRequest : LibraryBaseCommand
{
    public DeleteIssueArticleRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
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

public class DeleteArticleRequestHandler : RequestHandlerAsync<DeleteIssueArticleRequest>
{
    private readonly IIssueArticleRepository _articleRepository;

    public DeleteArticleRequestHandler(IIssueArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueArticleRequest> HandleAsync(DeleteIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var contents = await _articleRepository.GetArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        await _articleRepository.DeleteArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
