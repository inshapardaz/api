using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Page
{
    public class UploadBookPagesRequest : LibraryBaseCommand
    {
        public UploadBookPagesRequest(int libraryId, int bookId)
            : base(libraryId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

        public IEnumerable<FileModel> Files { get; set; }
    }

    public class UploadBookPagesHandler : RequestHandlerAsync<UploadBookPagesRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IConvertPdf _pdfConverter;
        private readonly IOpenZip _zipOpener;
        private readonly ILogger<UploadBookPagesHandler> _logger;

        public UploadBookPagesHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository,
            IFileStorage fileStorage, IConvertPdf pdfConverter, IOpenZip zipOpener, ILogger<UploadBookPagesHandler> logger)
        {
            _bookPageRepository = bookPageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _pdfConverter = pdfConverter;
            _zipOpener = zipOpener;
            _logger = logger;
        }

        public override async Task<UploadBookPagesRequest> HandleAsync(UploadBookPagesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogInformation("Upload files {Count} for book {bookId}", command.Files.Count(), command.BookId);
            var pageNumber = await _bookPageRepository.GetLastPageNumberForBook(command.LibraryId, command.BookId, cancellationToken);
            _logger.LogInformation("Last page number is {lastPageNumber}", pageNumber);

            IEnumerable<FileModel> files = new List<FileModel>();
            if (command.Files.Count() == 1 && command.Files.Single().MimeType == MimeTypes.Pdf)
            {
                var pages = _pdfConverter.ExtractImagePages(command.Files.Single().Contents);
                files = pages.Select(p => new FileModel()
                {
                    Contents = p.Value,
                    FileName = p.Key,
                    MimeType = MimeTypes.Jpg
                });
            }
            else if (command.Files.Count() == 1 && (command.Files.Single().MimeType == MimeTypes.Zip || command.Files.Single().MimeType == MimeTypes.CompressedFile))
            {
                files = _zipOpener.ExtractImages(command.Files.Single().Contents);
            }
            else
            {
                files = command.Files;
            }

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).Trim('.');
                var sequenceNumber = ++pageNumber;
                var url = await AddImageToFileStore(command.BookId, $"{sequenceNumber:0000}.{extension}", file.Contents, file.MimeType, cancellationToken);
                _logger.LogInformation("Added Image {Url} to filestore for book {bookId}", url, command.BookId);
                var fileModel = await _fileRepository.AddFile(new FileModel
                {
                    IsPublic = false,
                    FilePath = url,
                    DateCreated = DateTime.UtcNow,
                    FileName = file.FileName,
                    MimeType = file.MimeType
                }, cancellationToken);
                _logger.LogInformation("Added FileModel {id} for book {bookId} with path {FilePath}", fileModel.Id, command.BookId, file.FilePath);
                var bookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookId, pageNumber, string.Empty, fileModel.Id, null, cancellationToken);
                _logger.LogInformation("Added Book page {id} for book {bookId}", bookPage.ImageId, bookPage.BookId);

            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int bookId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(bookId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
        }

        private static string GetUniqueFileName(int bookId, string fileName)
        {
            return $"books/{bookId}/pages/{fileName}";
        }
    }
}
