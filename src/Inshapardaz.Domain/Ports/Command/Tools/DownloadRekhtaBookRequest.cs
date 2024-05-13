using Common;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book;
using Inshapardaz.Domain.Ports.Command.Library.Book.Page;
using Microsoft.Extensions.Options;
using Paramore.Brighter;
using RekhtaDownloader.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DownloadRekhtaBookRequest : RequestBase
{
    public DownloadRekhtaBookRequest(string url)
    {
        Url = url.Contains("?") ? url.Substring(0, url.IndexOf("?")) : url;
    }

    public string Url { get; init; }

    public bool CreatePdf { get; set; }

    public Result DownloadResult { get; set; }

    public class Result
    {
        public byte[] File { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }
    }
}

public class DownloadRekhtaBookRequestHandler : RequestHandlerAsync<DownloadRekhtaBookRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly Settings _settings;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    const bool _saveBook = false;
    public DownloadRekhtaBookRequestHandler(
        IAmACommandProcessor commandProcessor,
        IOptions<Settings> settings,
        IBookRepository bookRepository,
        IBookPageRepository bookPageRepository,
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        IAuthorRepository authorRepository)
    {
        _commandProcessor = commandProcessor;
        _settings = settings.Value;
        _bookRepository = bookRepository;
        _bookPageRepository = bookPageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
        _authorRepository = authorRepository;
    }

    public override async Task<DownloadRekhtaBookRequest> HandleAsync(DownloadRekhtaBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookBySource(_settings.DefaultLibraryId, command.Url, cancellationToken);
        if (book != null)
        {
            var bookContents = await _bookRepository.GetBookContents(_settings.DefaultLibraryId, book.Id, cancellationToken);
            if (command.CreatePdf)
            {
                if (bookContents.Any(c => c.MimeType == MimeTypes.Pdf))
                {
                    var file = await _fileRepository.GetFileById(bookContents.First(c => c.MimeType == MimeTypes.Pdf).FileId, cancellationToken);
                    if (file != null)
                    {
                        var contents = await _fileStorage.GetFile(file.FilePath, cancellationToken);
                        if (contents != null)
                        {
                            var result = new DownloadRekhtaBookRequest.Result()
                            {

                                FileName = Path.GetFileName(file.FileName),
                                MimeType = MimeTypes.Pdf,
                                File = contents
                            };

                            command.DownloadResult = result;

                            return await base.HandleAsync(command, cancellationToken);
                        }
                    }
                }
            }
            else
            {
                var pages = await _bookPageRepository.GetAllPagesByBook(_settings.DefaultLibraryId, book.Id, cancellationToken);
                if (pages != null && pages.Any(p => p.ImageId.HasValue))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var page in pages.Where(p => p.ImageId.HasValue))
                            {
                                var file = await _fileRepository.GetFileById(page.ImageId.Value, cancellationToken);
                                if (file != null && file.Contents != null)
                                {
                                    var contents = await _fileStorage.GetFile(file.FilePath, cancellationToken);

                                    var archiveFile = archive.CreateEntry(file.FileName);
                                    using (var entryStream = archiveFile.Open())
                                    {
                                        entryStream.Write(contents, 0, contents.Length);
                                    }
                                }
                                else
                                {
                                    await DeleteBookPages(book, pages, cancellationToken);
                                    await DownloadBook(book, command, cancellationToken);

                                    return await base.HandleAsync(command, cancellationToken);
                                }
                            }
                        }

                        var result = new DownloadRekhtaBookRequest.Result()
                        {

                            FileName = Path.GetFileName($"{book.Title}.zip"),
                            MimeType = MimeTypes.Zip,
                            File = memoryStream.ToArray()
                        };

                        command.DownloadResult = result;

                        return await base.HandleAsync(command, cancellationToken);

                    }
                }
            }
        }

        await DownloadBook(book, command, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task DownloadBook(BookModel book, DownloadRekhtaBookRequest command, CancellationToken cancellationToken)
    {
        var exporter = new RekhtaDownloader.BookExporter(new ConsoleLogger());
        var path = $"../data/downloads";
        var filePath = await exporter.DownloadBook(command.Url, 10, command.CreatePdf ? RekhtaDownloader.OutputType.Pdf : RekhtaDownloader.OutputType.Images, path, cancellationToken);
        var bookInfo = await exporter.GetBookInformation(command.Url, cancellationToken);

        try
        {
            if (book == null)
            {
                book = await CreateNewBook(bookInfo, command.Url, cancellationToken);
            }

            var result = new DownloadRekhtaBookRequest.Result()
            {

                FileName = Path.GetFileName(filePath),
                MimeType = command.CreatePdf ? MimeTypes.Pdf : MimeTypes.Zip
            };

            if (command.CreatePdf)
            {
                result.File = await System.IO.File.ReadAllBytesAsync(filePath);

                await SaveBookPdfContents(book, filePath, result.File, cancellationToken);
            }
            else
            {
                var zipFilePath = Path.Combine(new DirectoryInfo(filePath).Parent.FullName,
                    $"{new DirectoryInfo(filePath).Name}.zip");
                try
                {
                    zipFilePath.MakeSureFileDoesNotExist();
                    ZipFile.CreateFromDirectory(filePath, zipFilePath);
                    result.File = await System.IO.File.ReadAllBytesAsync(zipFilePath);
                }
                finally
                {
                    zipFilePath.TryDeleteFile();
                }

                await SaveBookPages(book, filePath, cancellationToken);
            }

            command.DownloadResult = result;
        }
        finally
        {
            if (command.CreatePdf)
                filePath.TryDeleteFile();
            else
                filePath.TryDeleteDirectory();
        }
    }


    private async Task DeleteBookPages(BookModel book, IEnumerable<BookPageModel> pages, CancellationToken cancellationToken)
    {
        if (!_saveBook) return;

        foreach (var page in pages.OrderByDescending(x => x.SequenceNumber))
        {
            await _bookPageRepository.DeletePage(_settings.DefaultLibraryId, book.Id, page.SequenceNumber, cancellationToken);
        }
    }
    public async Task<BookModel> CreateNewBook(BookInfo bookInfo, string source, CancellationToken cancellationToken)
    {
        if (!_saveBook) return null;

        var authorName = bookInfo.Authors?.FirstOrDefault() ?? "Unknown";
        var authors = await _authorRepository.FindAuthors(_settings.DefaultLibraryId, authorName, AuthorTypes.Writer, 1, 1, cancellationToken);
        AuthorModel author = null;

        if (!authors.Data.Any())
        {
            author = await _authorRepository.AddAuthor(_settings.DefaultLibraryId,
                new AuthorModel
                {
                    Name = authorName,
                    AuthorType = AuthorTypes.Writer
                }, cancellationToken);
        }
        else
        {
            author = authors.Data.First();
        }

        var book = await _bookRepository.AddBook(_settings.DefaultLibraryId, new BookModel
        {
            Title = bookInfo.Title,
            Authors = new List<AuthorModel>() { author },
            Language = "ur",
            Source = source
        }, null, cancellationToken);

        if (bookInfo.Image != null && bookInfo.Image.Length > 0)
        {
            var updateImgRequest = new UpdateBookImageRequest(_settings.DefaultLibraryId, book.Id, null)
            {
                Image = new FileModel
                {
                    MimeType = MimeTypes.Jpg,
                    Contents = bookInfo.Image,
                    FileName = "image.jpg"
                }
            };

            await _commandProcessor.SendAsync(updateImgRequest);
        }

        return book;
    }

    public async Task SaveBookPdfContents(BookModel book, string filePath, byte[] contents, CancellationToken cancellationToken)
    {
        if (!_saveBook) return;

        var cmdAddBookContent = new AddBookContentRequest(_settings.DefaultLibraryId, book.Id, null, MimeTypes.Pdf, null)
        {
            Content = new FileModel
            {
                MimeType = MimeTypes.Pdf,
                Contents = contents,
                FileName = Path.GetFileName(filePath)
            }
        };
        await _commandProcessor.SendAsync(cmdAddBookContent, cancellationToken: cancellationToken);

        var amdAddBookPages = new UploadBookPagesRequest(_settings.DefaultLibraryId, book.Id)
        {
            Files = new[] { new FileModel
                    {
                        MimeType = MimeTypes.Pdf,
                        Contents = contents,
                        FileName = Path.GetFileName(filePath)
                    }}
        };

        await _commandProcessor.SendAsync(amdAddBookPages, cancellationToken: cancellationToken);
    }

    public async Task SaveBookPages(BookModel book, string filePath, CancellationToken cancellationToken)
    {
        if (!_saveBook) return;

        var _files = Directory.EnumerateFiles(filePath)
                 .OrderBy(filename => filename)
                 .Select(f => new FileModel
                 {
                     MimeType = MimeTypes.Jpg,
                     Contents = System.IO.File.ReadAllBytes(f),
                     FileName = Path.GetFileName(f)
                 }).ToList();

        var cmd = new UploadBookPagesRequest(_settings.DefaultLibraryId, book.Id)
        {
            Files = _files
        };

        await _commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
    }
}
