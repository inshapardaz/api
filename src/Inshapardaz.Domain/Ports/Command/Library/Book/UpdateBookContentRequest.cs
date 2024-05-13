using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookContentRequest : LibraryBaseCommand
{
    public UpdateBookContentRequest(int libraryId, int bookId, int contentId, string language, string mimeType, int? accountId)
        : base(libraryId)
    {
        BookId = bookId;
        ContentId = contentId;
        Language = language;
        MimeType = mimeType;
        AccountId = accountId;
    }

    public int BookId { get; }
    public int ContentId { get; }
    public string Language { get; }
    public string MimeType { get; }
    public int? AccountId { get; }
    public FileModel Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public BookContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookFileRequestHandler : RequestHandlerAsync<UpdateBookContentRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public UpdateBookFileRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _bookRepository = bookRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<UpdateBookContentRequest> HandleAsync(UpdateBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            var bookContent = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
            if (bookContent != null)
            {
                if (!string.IsNullOrWhiteSpace(bookContent.ContentUrl))
                {
                    await _fileStorage.TryDeleteFile(bookContent.ContentUrl, cancellationToken);
                }

                var url = await StoreFile(command.BookId, command.Content.FileName, command.Content.Contents, cancellationToken);
                bookContent.ContentUrl = url;
                bookContent.MimeType = command.MimeType;
                bookContent.Language = command.Language;
                await _bookRepository.UpdateBookContent(command.LibraryId,
                                                        command.BookId,
                                                        command.ContentId,
                                                        command.Language,
                                                        command.MimeType,
                                                        url, cancellationToken);

                command.Result.Content = bookContent;
            }
            else
            {
                var url = await StoreFile(command.BookId, command.Content.FileName, command.Content.Contents, cancellationToken);
                command.Content.FilePath = url;
                command.Content.IsPublic = book.IsPublic;
                var file = await _fileRepository.AddFile(command.Content, cancellationToken);
                var contentId = await _bookRepository.AddBookContent(command.BookId, file.Id, command.Language, command.MimeType, cancellationToken);

                command.Result.HasAddedNew = true;
                command.Result.Content = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken); ;
            }
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
