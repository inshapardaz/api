using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddBookPageBulkRequest : LibraryBaseCommand
    {
        public AddBookPageBulkRequest(int libraryId, int bookId)
            : base(libraryId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

        public FileModel ZipFile { get; set; }
    }

    public class AddBookPageBulkRequestHandler : RequestHandlerAsync<AddBookPageBulkRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public AddBookPageBulkRequestHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookPageRepository = bookPageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<AddBookPageBulkRequest> HandleAsync(AddBookPageBulkRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var pageNumber = await _bookPageRepository.GetLastPageNumberForBook(command.LibraryId, command.BookId, cancellationToken);
            using (Stream stream = new MemoryStream(command.ZipFile.Contents))
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var extention = Path.GetExtension(entry.FullName);
                    if (extention.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                        extention.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        extention.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        var file = entry.Open();
                        byte[] fileContents = null;
                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            fileContents = memoryStream.ToArray();
                        }

                        var sequenceNumber = pageNumber++;
                        var url = await AddImageToFileStore(command.BookId, sequenceNumber, $"{sequenceNumber}{extention}", fileContents, cancellationToken);
                        var fileModel = await _fileRepository.AddFile(new FileModel
                        {
                            IsPublic = false,
                            FilePath = url,
                            DateCreated = DateTime.UtcNow,
                            FileName = $"{sequenceNumber}{extention}",
                            MimeType = extention == ".png" ? "image/png" : "image/jpeg"
                        }, cancellationToken);
                        var bookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookId, pageNumber++, string.Empty, fileModel.Id, cancellationToken);
                    }
                }
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
