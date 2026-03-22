using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class InternalUploadBookPagesRequest(int libraryId, int bookId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;

    public IEnumerable<FileModel> Files { get; set; }
}

public class InternalUploadBookPagesRequestHandler(
    IBookPageRepository bookPageRepository,
    IConvertPdf pdfConverter,
    IOpenZip zipOpener,
    ILogger<UploadBookPagesHandler> logger,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<InternalUploadBookPagesRequest>
{
    public override async Task<InternalUploadBookPagesRequest> HandleAsync(InternalUploadBookPagesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        logger.LogInformation("Upload files {Count} for book {bookId}", command.Files.Count(), command.BookId);
        var pageNumber = await bookPageRepository.GetLastPageNumberForBook(command.LibraryId, command.BookId, cancellationToken);
        logger.LogInformation("Last page number is {lastPageNumber}", pageNumber);

        IEnumerable<FileModel> files = new List<FileModel>();
        if (command.Files.Count() == 1 && command.Files.Single().MimeType == MimeTypes.Pdf)
        {
            var pages = pdfConverter.ExtractImagePages(command.Files.Single().Contents);
            files = pages.Select(p => new FileModel()
            {
                Contents = p.Value,
                FileName = p.Key,
                MimeType = MimeTypes.Jpg
            });
        }
        else if (command.Files.Count() == 1 && (command.Files.Single().MimeType == MimeTypes.Zip || command.Files.Single().MimeType == MimeTypes.CompressedFile))
        {
            files = zipOpener.ExtractImages(command.Files.Single().Contents);
        }
        else
        {
            files = command.Files;
        }

        foreach (var file in files)
        {
            var sequenceNumber = ++pageNumber;
            var fileName = FilePathHelper.GetBookPageFileName(file.FileName);
            var filePath = FilePathHelper.GetBookPageFilePath(command.BookId, fileName);

            var saveImageCommand = new SaveFileCommand(fileName, filePath, file.Contents)
            {
                MimeType = file.MimeType
            };

            await commandProcessor.SendAsync(saveImageCommand, cancellationToken: cancellationToken);

            logger.LogInformation("Added FileModel {id} for book {bookId} with path {FilePath}", saveImageCommand.Result.Id, command.BookId, saveImageCommand.Result.FilePath);
            var newBookPage = new BookPageModel
            {
                BookId = command.BookId, 
                SequenceNumber = pageNumber,
                ImageId = saveImageCommand.Result.Id
            };
            var bookPage = await bookPageRepository.AddPage(command.LibraryId, newBookPage, cancellationToken);
            logger.LogInformation("Added Book page {id} for book {bookId}", bookPage.ImageId, bookPage.BookId);

        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
