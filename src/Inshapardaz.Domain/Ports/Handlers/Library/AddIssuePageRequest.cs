using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddIssuePageRequest : LibraryBaseCommand
    {
        public AddIssuePageRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int? accountId, IssuePageModel issue)
        : base(libraryId)
        {
            AccountId = accountId;
            IssuePage = issue;
            IssuePage.VolumeNumber = volumeNumber;
            IssuePage.PeriodicalId = periodicalId;
            IssuePage.IssueNumber = issueNumber;
        }

        public int? AccountId { get; }
        public IssuePageModel IssuePage { get; }

        public IssuePageModel Result { get; set; }

        public bool IsAdded { get; set; }
    }

    public class AddIssuePageRequestHandler : RequestHandlerAsync<AddIssuePageRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IIssuePageRepository _issuePageRepository;

        public AddIssuePageRequestHandler(IIssueRepository issueRepository,
                                         IIssuePageRepository issuePageRepository)
        {
            _issueRepository = issueRepository;
            _issuePageRepository = issuePageRepository;
        }

        public override async Task<AddIssuePageRequest> HandleAsync(AddIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var Issue = await _issueRepository.GetIssue(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, cancellationToken);
            if (Issue == null)
            {
                throw new BadRequestException();
            }

            var existingIssuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, cancellationToken);

            if (existingIssuePage == null)
            {
                command.Result = await _issuePageRepository.AddPage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.Text, 0, command.IssuePage.ArticleNumber, cancellationToken);
                command.IsAdded = true;
            }
            else
            {
                command.Result = await _issuePageRepository.UpdatePage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.Text, 0, command.IssuePage.ArticleNumber, command.IssuePage.Status, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
