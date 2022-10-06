using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteArticleContentRequest : LibraryBaseCommand
    {
        public DeleteArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language)
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

    public class DeleteArticleContentRequestHandler : RequestHandlerAsync<DeleteArticleContentRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public DeleteArticleContentRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<DeleteArticleContentRequest> HandleAsync(DeleteArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _articleRepository.GetArticleContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, cancellationToken);

            if (content != null)
            {
                await _articleRepository.DeleteArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
