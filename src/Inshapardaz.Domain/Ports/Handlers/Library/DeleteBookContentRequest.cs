using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteBookContentRequest : BookRequest
    {
        public DeleteBookContentRequest(int libraryId, int bookId, string language, string mimeType)
            : base(libraryId, bookId)
        {
            Language = language;
            MimeType = mimeType;
        }

        public string Language { get; }
        public string MimeType { get; }
    }

    public class DeleteBookContentRequestHandler : RequestHandlerAsync<DeleteBookContentRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteBookContentRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteBookContentRequest> HandleAsync(DeleteBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.Language, command.MimeType, cancellationToken);
            if (content != null)
            {
                await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                await _bookRepository.DeleteBookContent(command.LibraryId, command.BookId, command.Language, command.MimeType, cancellationToken);
                await _fileRepository.DeleteFile(content.FileId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
