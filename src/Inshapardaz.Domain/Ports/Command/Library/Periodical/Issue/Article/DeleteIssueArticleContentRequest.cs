using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class DeleteIssueArticleContentRequest : LibraryBaseCommand
{
    public DeleteIssueArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = articleId;
        Language = language;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }
    public string Language { get; }
}

public class DeleteArticleContentRequestHandler : RequestHandlerAsync<DeleteIssueArticleContentRequest>
{
    private readonly IIssueArticleRepository _articleRepository;
    private readonly IAmACommandProcessor _commandProcessor;


    public DeleteArticleContentRequestHandler(IIssueArticleRepository articleRepository, IAmACommandProcessor commandProcessor)
    {
        _articleRepository = articleRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueArticleContentRequest> HandleAsync(DeleteIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await _articleRepository.GetIssueArticleContent(command.LibraryId, new IssueArticleContentModel
        {
            PeriodicalId = command.PeriodicalId,
            VolumeNumber = command.VolumeNumber,
            IssueNumber = command.IssueNumber,
            SequenceNumber = command.SequenceNumber,
            Language = command.Language
        }, cancellationToken);

        if (content != null)
        {
            await _commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            await _articleRepository.DeleteIssueArticleContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
