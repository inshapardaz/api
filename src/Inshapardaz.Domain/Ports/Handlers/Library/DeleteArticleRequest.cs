using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteArticleRequest : LibraryBaseCommand
    {
        public DeleteArticleRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            ArticleId = articleId;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int ArticleId { get; }
    }

    public class DeleteArticleRequestHandler : RequestHandlerAsync<DeleteArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteArticleRequestHandler(IArticleRepository articleRepository, IFileStorage fileStorage)
        {
            _articleRepository = articleRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var contents = await _articleRepository.GetArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, cancellationToken);

            await _articleRepository.DeleteArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, cancellationToken);

            // TODO : Delete the files

            foreach (var content in contents)
            {
            }
            //if (!string.IsNullOrWhiteSpace(filePath))
            //    await _fileStorage.TryDeleteFile(filePath, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
