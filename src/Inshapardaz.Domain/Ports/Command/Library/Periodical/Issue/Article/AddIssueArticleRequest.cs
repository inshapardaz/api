using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class AddIssueArticleRequest(
    int libraryId,
    int peridicalId,
    int volumeNumber,
    int issueNumber,
    IssueArticleModel article)
    : LibraryBaseCommand(libraryId)
{
    public IssueArticleModel Result { get; set; }
    public int PeridicalId { get; } = peridicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public IssueArticleModel Article { get; } = article;
}

public class AddArticleRequestHandler(IIssueRepository issueRepository, IIssueArticleRepository articleRepository)
    : RequestHandlerAsync<AddIssueArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssueArticleRequest> HandleAsync(AddIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeridicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }
        
        command.Result = await articleRepository.AddIssueArticle(
            command.LibraryId, 
            command.PeridicalId, 
            command.VolumeNumber, 
            command.IssueNumber, 
            command.Article, 
            cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
