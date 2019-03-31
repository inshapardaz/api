using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteAuthorRequest : RequestBase
    {
        public DeleteAuthorRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }
    }

    public class DeleteAuthorRequestHandler : RequestHandlerAsync<DeleteAuthorRequest>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStore;

        public DeleteAuthorRequestHandler(IAuthorRepository authorRepository, IFileRepository fileRepository, IFileStorage fileStore)
        {
            _authorRepository = authorRepository;
            _fileRepository = fileRepository;
            _fileStore = fileStore;
        }

        public override async Task<DeleteAuthorRequest> HandleAsync(DeleteAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _authorRepository.GetAuthorById(command.AuthorId, cancellationToken);
            if (author != null && author.ImageId > 0)
            {
                var image = await _fileRepository.GetFileById(author.ImageId, true, cancellationToken);
                if (string.IsNullOrWhiteSpace(image.FilePath))
                {
                    await _fileStore.TryDeleteFile(image.FilePath, cancellationToken);
                    await _fileRepository.DeleteFile(image.Id, cancellationToken);
                }
            }
            await _authorRepository.DeleteAuthor(command.AuthorId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}