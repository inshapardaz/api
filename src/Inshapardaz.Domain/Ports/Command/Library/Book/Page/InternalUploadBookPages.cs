using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class InternalUploadBookPagesRequest : LibraryBaseCommand
{
    public InternalUploadBookPagesRequest(int libraryId, int bookId)
        : base(libraryId)
    {
        BookId = bookId;
    }

    public int BookId { get; }

    public IEnumerable<FileModel> Files { get; set; }
}

public class InternalUploadBookPagesRequestHandler : RequestHandlerAsync<InternalUploadBookPagesRequest>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IConvertPdf _pdfConverter;
    private readonly IOpenZip _zipOpener;
    private readonly ILogger<UploadBookPagesHandler> _logger;

    public InternalUploadBookPagesRequestHandler(IBookPageRepository bookPageRepository,
        IConvertPdf pdfConverter,
        IOpenZip zipOpener,
        ILogger<UploadBookPagesHandler> logger,
        IAmACommandProcessor commandProcessor)
    {
        _bookPageRepository = bookPageRepository;
        _pdfConverter = pdfConverter;
        _zipOpener = zipOpener;
        _logger = logger;
        _commandProcessor = commandProcessor;
    }

    public override async Task<InternalUploadBookPagesRequest> HandleAsync(InternalUploadBookPagesRequest command, CancellationToken cancellationToken = new CancellationToken())
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
            var sequenceNumber = ++pageNumber;
            var fileName = FilePathHelper.GetBookPageFileName(file.FileName);
            var filePath = FilePathHelper.GetBookPageFilePath(command.BookId, fileName);

            var saveImageCommand = new SaveFileCommand(fileName, filePath, file.Contents)
            {
                MimeType = file.MimeType
            };

            await _commandProcessor.SendAsync(saveImageCommand, cancellationToken: cancellationToken);

            _logger.LogInformation("Added FileModel {id} for book {bookId} with path {FilePath}", saveImageCommand.Result.Id, command.BookId, saveImageCommand.Result.FilePath);
            var newBookPage = new BookPageModel
            {
                BookId = command.BookId, 
                SequenceNumber = pageNumber,
                ImageId = saveImageCommand.Result.Id
            };
            var bookPage = await _bookPageRepository.AddPage(command.LibraryId, newBookPage, cancellationToken);
            _logger.LogInformation("Added Book page {id} for book {bookId}", bookPage.ImageId, bookPage.BookId);

        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
