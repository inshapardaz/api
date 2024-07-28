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

public class DeleteIssueArticleRequestHandler : RequestHandlerAsync<DeleteIssueArticleRequest>
{
    private readonly IIssueArticleRepository _articleRepository;
    private readonly IAmACommandProcessor _commandProcessor;
    public DeleteIssueArticleRequestHandler(IIssueArticleRepository articleRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _articleRepository = articleRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueArticleRequest> HandleAsync(DeleteIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _articleRepository.GetIssueArticle(command.PeriodicalId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber,
            command.SequenceNumber, cancellationToken);
        if (issue is not null)
        {
            var contents = await _articleRepository.GetIssueArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            foreach (var content in contents)  
            {
                await _commandProcessor.SendAsync(new DeleteIssueArticleContentRequest(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, content.Language), cancellationToken: cancellationToken);
            }
            await _articleRepository.DeleteIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
