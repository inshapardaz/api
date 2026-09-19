using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class UpsertIssueRatingRequest(int libraryId, int accountId, int periodicalId, int volumeNumber, int issueNumber, RatingModel rating)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public RatingModel Rating { get; } = rating;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public RatingModel Rating { get; set; }
    }
}

public class UpsertIssueRatingRequestHandler(IIssueRepository issueRepository)
    : RequestHandlerAsync<UpsertIssueRatingRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpsertIssueRatingRequest> HandleAsync(UpsertIssueRatingRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.Rating.Value < 1 || command.Rating.Value > 5)
        {
            throw new BadRequestException("Rating value must be between 1 and 5");
        }

        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue != null)
        {
            var result = await issueRepository.UpsertIssueRating(
                command.LibraryId, command.AccountId, issue.Id, command.Rating, cancellationToken);
            command.Result = new UpsertIssueRatingRequest.RequestResult
            {
                Rating = result
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
