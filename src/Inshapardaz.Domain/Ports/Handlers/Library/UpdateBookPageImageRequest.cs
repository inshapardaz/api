using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateBookPageImageRequest : LibraryAuthorisedCommand
    {
        public UpdateBookPageImageRequest(ClaimsPrincipal claims, int libraryId, int bookId, int sequenceNumber)
            : base(claims, libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
        }

        public int BookId { get; }

        public int SequenceNumber { get; }

        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookPageImageRequestHandler : RequestHandlerAsync<UpdateBookPageImageRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateBookPageImageRequestHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookPageRepository = bookPageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<UpdateBookPageImageRequest> HandleAsync(UpdateBookPageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

            if (bookPage == null)
            {
                throw new NotFoundException();
            }

            if (bookPage.ImageId.HasValue)
            {
                command.Image.Id = bookPage.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(bookPage.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(command.BookId, command.SequenceNumber, command.Image.FileName, command.Image.Contents, cancellationToken);

                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = bookPage.ImageId.Value;
            }
            else
            {
                command.Image.Id = default(int);
                var url = await AddImageToFileStore(command.BookId, command.SequenceNumber, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                await _bookPageRepository.UpdatePageImage(command.LibraryId, command.BookId, command.SequenceNumber, command.Result.File.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int bookId, int sequenceNumber, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(bookId, sequenceNumber, fileName);
            return await _fileStorage.StoreImage(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int bookId, int sequenceNumber, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"books/{bookId}/pages/{sequenceNumber}_{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
