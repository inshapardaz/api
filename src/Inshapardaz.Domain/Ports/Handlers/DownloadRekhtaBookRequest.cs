using Common;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library.Book;
using Inshapardaz.Domain.Ports.Handlers.Library.Book.Page;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Lucene.Net.Search;
using Microsoft.IdentityModel.Tokens;
using Paramore.Brighter;
using RekhtaDownloader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models
{
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


        public DownloadRekhtaBookRequestHandler(
            IAmACommandProcessor commandProcessor,
            Settings settings,
            IBookRepository bookRepository,
            IBookPageRepository bookPageRepository,
            IFileRepository fileRepository,
            IFileStorage fileStorage,
            IAuthorRepository authorRepository)
        {
            _commandProcessor = commandProcessor;
            _settings = settings;
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
                                    if (file != null)
                                    {
                                        var contents = await _fileStorage.GetFile(file.FilePath, cancellationToken);

                                        var archiveFile = archive.CreateEntry(file.FileName);
                                        using (var entryStream = archiveFile.Open())
                                        {
                                            entryStream.Write(contents, 0, contents.Length);
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
            }

            var exporter = new RekhtaDownloader.BookExporter(new ConsoleLogger());
            var path = $"../data/downloads/{Guid.NewGuid():N}";
            try
            {
                var filePath = await exporter.DownloadBook(command.Url, 10, command.CreatePdf ? RekhtaDownloader.OutputType.Pdf : RekhtaDownloader.OutputType.Images, path, cancellationToken);
                var bookInfo = await exporter.GetBookInformation(command.Url, cancellationToken);

                if (book == null)
                {
                    book = await CreateNewBook(bookInfo, command.Url,  cancellationToken);
                }

                var result = new DownloadRekhtaBookRequest.Result()
                {

                    FileName = Path.GetFileName(filePath),
                    MimeType = command.CreatePdf ? MimeTypes.Pdf : MimeTypes.Zip
                };

                if (command.CreatePdf)
                {
                    result.File = await File.ReadAllBytesAsync(filePath);

                    var cmdAddBookContent = new AddBookContentRequest(_settings.DefaultLibraryId, book.Id, null, MimeTypes.Pdf, null)
                    {
                        Content = new FileModel
                        {
                            MimeType = MimeTypes.Pdf,
                            Contents = result.File,
                            FileName = Path.GetFileName(filePath)
                        }
                    };
                    await _commandProcessor.SendAsync(cmdAddBookContent, cancellationToken: cancellationToken);
                }
                else
                {
                    ZipFile.CreateFromDirectory(filePath, $"{path}.zip");
                    result.File = await File.ReadAllBytesAsync($"{path}.zip");

                    var _files = Directory.EnumerateFiles(filePath)
                     .OrderByDescending(filename => filename)
                     .Select(f => new FileModel {  
                         MimeType = MimeTypes.Jpg, 
                         Contents = File.ReadAllBytes(f),
                         FileName = Path.GetFileName(f)
                     }).ToList();

                    var cmd = new UploadBookPages(_settings.DefaultLibraryId, book.Id)
                    {
                        Files = _files
                    };

                    await _commandProcessor.SendAsync(cmd);
                }

                command.DownloadResult = result;

                return await base.HandleAsync(command, cancellationToken);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        public async Task<BookModel> CreateNewBook(BookInfo bookInfo, string source, CancellationToken cancellationToken)
        {
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
    }
}
