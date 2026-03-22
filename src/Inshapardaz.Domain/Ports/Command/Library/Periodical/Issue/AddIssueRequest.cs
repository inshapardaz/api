using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class AddIssueRequest(int libraryId, int periodicalId, IssueModel issue) : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;

    public IssueModel Issue { get; } = issue;

    public IssueModel Result { get; set; }

    public class AddIssueRequestHandler(IPeriodicalRepository periodicalRepository, IIssueRepository issueRepository)
        : RequestHandlerAsync<AddIssueRequest>
    {
        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<AddIssueRequest> HandleAsync(AddIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {

            var periodical = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);

            if (periodical == null)
            {
                throw new BadRequestException();
            }

            var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.Issue.VolumeNumber, command.Issue.IssueNumber, cancellationToken);

            if (issue != null)
            {
                throw new ConflictException();
            }

            command.Result = await issueRepository.AddIssue(command.LibraryId, command.PeriodicalId, command.Issue, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
