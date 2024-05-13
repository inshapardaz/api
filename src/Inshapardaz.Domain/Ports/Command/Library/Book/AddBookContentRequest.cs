using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class AddBookContentRequest : LibraryBaseCommand
{
    public AddBookContentRequest(int libraryId, int bookId, string language, string mimeType, int? accountId)
        : base(libraryId)
    {
        BookId = bookId;
        Language = language;
        MimeType = mimeType;
        AccountId = accountId;
    }

    public int BookId { get; }

    public string Language { get; }
    public string MimeType { get; }
    public int? AccountId { get; }
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

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddBookContentRequest> HandleAsync(AddBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);

        if (book != null)
        {
            var status = (await _bookRepository.GetBookPageSummary(command.LibraryId, new[] { book.Id }, cancellationToken)).FirstOrDefault();

            if (status != null)
            {
                book.PageStatus = status.Statuses;
                if (status.Statuses.Any(s => s.Status == EditingStatus.Completed))
                {
                    decimal completedPages = status.Statuses.Single(s => s.Status == EditingStatus.Completed).Count;
                    decimal totalPages = status.Statuses.Sum(s => s.Count);
                    book.Progress = completedPages / totalPages * 100;
                }
                else
                {
                    book.Progress = 0.0M;
                }
            }

            var url = await StoreFile(book.Id, command.Content.FileName, command.Content.Contents, cancellationToken);
            command.Content.FilePath = url;
            command.Content.IsPublic = true;
            var file = await _fileRepository.AddFile(command.Content, cancellationToken);
            var contentId = await _bookRepository.AddBookContent(book.Id, file.Id, command.Language, command.MimeType, cancellationToken);

            command.Result = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken); ;
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
