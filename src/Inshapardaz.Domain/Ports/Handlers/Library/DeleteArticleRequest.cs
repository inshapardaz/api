using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteArticleRequest : LibraryBaseCommand
    {
        public DeleteArticleRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
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

    public class DeleteArticleRequestHandler : RequestHandlerAsync<DeleteArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public DeleteArticleRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var contents = await _articleRepository.GetArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            await _articleRepository.DeleteArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
