using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateBookImageRequest : RequestBase
    {
        public UpdateBookImageRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

        public File Image { get; set; }


        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public File File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookImageRequestHandler : RequestHandlerAsync<UpdateBookImageRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public UpdateBookImageRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<UpdateBookImageRequest> HandleAsync(UpdateBookImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException();
            }

            if (book.ImageId != 0)
            {
                command.Image.Id = book.ImageId;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = book.ImageId;
            }
            else
            {
                command.Image.Id = default(int);
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                book.ImageId = command.Result.File.Id;
                await _bookRepository.UpdateBook(book, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
