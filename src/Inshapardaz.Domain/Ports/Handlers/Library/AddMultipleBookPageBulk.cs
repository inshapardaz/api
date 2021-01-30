using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddMultipleBookPageBulk : LibraryBaseCommand
    {
        public AddMultipleBookPageBulk(int libraryId, int bookId)
            : base(libraryId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

        public IEnumerable<FileModel> Files { get; set; }
    }

    public class AddMultipleBookPageBulkHandler : RequestHandlerAsync<AddMultipleBookPageBulk>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public AddMultipleBookPageBulkHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookPageRepository = bookPageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<AddMultipleBookPageBulk> HandleAsync(AddMultipleBookPageBulk command, CancellationToken cancellationToken = new CancellationToken())
        {
            var pageNumber = await _bookPageRepository.GetLastPageNumberForBook(command.LibraryId, command.BookId, cancellationToken);

            foreach (var file in command.Files)
            {
                var extension = Path.GetExtension(file.FileName).Trim('.');
                var sequenceNumber = ++pageNumber;
                var url = await AddImageToFileStore(command.BookId, sequenceNumber, $"{sequenceNumber}{extension}", file.Contents, cancellationToken);
                var fileModel = await _fileRepository.AddFile(new FileModel
                {
                    IsPublic = false,
                    FilePath = url,
                    DateCreated = DateTime.UtcNow,
                    FileName = file.FileName,
                    MimeType = file.MimeType
                }, cancellationToken);
                var bookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookId, pageNumber, string.Empty, fileModel.Id, cancellationToken);
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
