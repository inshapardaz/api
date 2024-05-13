using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Article
{
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

        public DeleteArticleContentRequestHandler(IIssueArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<DeleteIssueArticleContentRequest> HandleAsync(DeleteIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _articleRepository.GetArticleContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, cancellationToken);

            if (content != null)
            {
                await _articleRepository.DeleteArticleContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
