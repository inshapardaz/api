using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class AssignIssueArticleToUserRequest : LibraryBaseCommand
{
    public AssignIssueArticleToUserRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, bool isAdmin = false)
    : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        AccountId = accountId;
        IsAdmin = isAdmin;
    }

    public IssueArticleModel Result { get; set; }
    public int PeriodicalId { get; set; }
    public int VolumeNumber { get; set; }
    public int IssueNumber { get; set; }
    public int SequenceNumber { get; set; }
    public int? AccountId { get; private set; }
    public bool IsAdmin { get; }
}

public class AssignArticleToUserRequestHandler : RequestHandlerAsync<AssignIssueArticleToUserRequest>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IIssueArticleRepository _articleRepository;

    public AssignArticleToUserRequestHandler(IAccountRepository accountRepository,
                                     IIssueArticleRepository articleRepository)
    {
        _accountRepository = accountRepository;
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignIssueArticleToUserRequest> HandleAsync(AssignIssueArticleToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (!command.IsAdmin)
        {
            var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId.Value, cancellationToken);
            if (account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned article");
            }
        }

        var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (article == null)
        {
            throw new BadRequestException();
        }

        if (article.Status == EditingStatus.Available || article.Status == EditingStatus.Typing)
        {
            command.Result = await _articleRepository.UpdateWriterAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (article.Status == EditingStatus.Typed || article.Status == EditingStatus.InReview)
        {
            command.Result = await _articleRepository.UpdateReviewerAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Article does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
