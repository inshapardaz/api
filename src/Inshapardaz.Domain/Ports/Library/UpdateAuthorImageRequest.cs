using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateAuthorImageRequest : RequestBase
    {
        public UpdateAuthorImageRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }

        public File Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public File File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateAuthorImageRequestHandler : RequestHandlerAsync<UpdateAuthorImageRequest>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IFileRepository _fileRepository;

        public UpdateAuthorImageRequestHandler(IAuthorRepository authorRepository, IFileRepository fileRepository)
        {
            _authorRepository = authorRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<UpdateAuthorImageRequest> HandleAsync(UpdateAuthorImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _authorRepository.GetAuthorById(command.AuthorId, cancellationToken);

            if (author == null)
            {
                throw new NotFoundException();
            }

            if (author.ImageId != 0)
            {
                command.Image.Id = author.ImageId;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = author.ImageId;
            }
            else
            {
                command.Image.Id = default(int);
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken); 
                command.Result.HasAddedNew = true;

                author.ImageId = command.Result.File.Id;
                await _authorRepository.UpdateAuthor(author, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
