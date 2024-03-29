﻿using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Author
{
    public class DeleteAuthorRequest : LibraryBaseCommand
    {
        public DeleteAuthorRequest(int libraryId, int authorId)
            : base(libraryId)
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
            var author = await _authorRepository.GetAuthorById(command.LibraryId, command.AuthorId, cancellationToken);
            if (author != null)
            {
                if (author.ImageId.HasValue)
                {
                    var image = await _fileRepository.GetFileById(author.ImageId.Value, cancellationToken);
                    if (image != null && !string.IsNullOrWhiteSpace(image.FilePath))
                    {
                        await _fileStore.TryDeleteImage(image.FilePath, cancellationToken);
                        await _fileRepository.DeleteFile(image.Id, cancellationToken);
                    }
                }
                await _authorRepository.DeleteAuthor(command.LibraryId, command.AuthorId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
