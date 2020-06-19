using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FileModel = Inshapardaz.Domain.Models.FileModel;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddBookContentRequest : LibraryAuthorisedCommand
    {
        public AddBookContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, string language, string mimeType)
            : base(claims, libraryId)
        {
            BookId = bookId;
            Language = language;
            MimeType = mimeType;
        }

        public int BookId { get; }

        public string Language { get; }
        public string MimeType { get; }
        public FileModel Content { get; set; }

        public BookContentModel Result { get; set; }
    }

    public class AddBookFileRequestHandler : RequestHandlerAsync<AddBookContentRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public AddBookFileRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddBookContentRequest> HandleAsync(AddBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);

            if (book != null)
            {
                var url = await StoreFile(book.Id, command.Content.FileName, command.Content.Contents, cancellationToken);
                command.Content.FilePath = url;
                command.Content.IsPublic = true;
                var file = await _fileRepository.AddFile(command.Content, cancellationToken);
                await _bookRepository.AddBookContent(book.Id, file.Id, command.Language, command.MimeType, cancellationToken);

                command.Result = await _bookRepository.GetBookContent(book.LibraryId, book.Id, command.Language, command.MimeType, cancellationToken); ;
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> StoreFile(int bookId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(bookId, fileName);
            return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int bookId, string fileName)
        {
            var fileNameWithoutExtension = Path.GetExtension(fileName).Trim('.');
            return $"books/{bookId}/{Guid.NewGuid():N}.{fileNameWithoutExtension}";
        }
    }
}
