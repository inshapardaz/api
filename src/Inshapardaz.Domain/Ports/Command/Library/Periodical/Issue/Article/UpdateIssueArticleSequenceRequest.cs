using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class UpdateIssueArticleSequenceRequest : LibraryBaseCommand
{
    public UpdateIssueArticleSequenceRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<IssueArticleModel> articles)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        Articles = articles;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public IEnumerable<IssueArticleModel> Articles { get; }

    public IEnumerable<IssueArticleModel> Result { get; set; }
}

public class UpdateIssueArticleSequenceRequestHandler : RequestHandlerAsync<UpdateIssueArticleSequenceRequest>
{
    private readonly IIssueArticleRepository _articleRepository;

    public UpdateIssueArticleSequenceRequestHandler(IIssueArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueArticleSequenceRequest> HandleAsync(UpdateIssueArticleSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var articles = await _articleRepository.GetArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (articles != null)
        {
            foreach (var a1 in command.Articles)
            {
                var a2 = articles.SingleOrDefault(x => x.Id == a1.Id);
                if (a2 == null)
                {
                    throw new BadRequestException("Resource out of date.");
                }
                a2.SequenceNumber = a1.SequenceNumber;
            }

            await _articleRepository.UpdateArticleSequence(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, articles, cancellationToken);
            command.Result = await _articleRepository.GetArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
