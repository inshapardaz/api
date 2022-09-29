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
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteArticleRequestHandler(IArticleRepository articleRepository, IFileStorage fileStorage, IFileRepository fileRepository)
        {
            _articleRepository = articleRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
        }

        public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var contents = await _articleRepository.GetArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            await _articleRepository.DeleteArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            foreach (var content in contents)
            {
                var image = await _fileRepository.GetFileById(content.FileId, cancellationToken);
                if (image != null && !string.IsNullOrWhiteSpace(image.FilePath))
                {
                    await _fileStorage.TryDeleteImage(image.FilePath, cancellationToken);
                    await _fileRepository.DeleteFile(image.Id, cancellationToken);
                }

            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
