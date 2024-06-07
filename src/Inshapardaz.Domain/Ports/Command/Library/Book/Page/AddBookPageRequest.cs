using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class AddBookPageRequest : LibraryBaseCommand
{
    public AddBookPageRequest(int libraryId, int bookId, int? accountId, BookPageModel book)
    : base(libraryId)
    {
        AccountId = accountId;
        BookPage = book;
        BookPage.BookId = bookId;
    }

    public int? AccountId { get; }
    public BookPageModel BookPage { get; }

    public BookPageModel Result { get; set; }

    public bool IsAdded { get; set; }
}

public class AddBookPageRequestHandler : RequestHandlerAsync<AddBookPageRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public AddBookPageRequestHandler(IBookRepository bookRepository,
                                     IBookPageRepository bookPageRepository,
                                     IFileRepository fileRepository, 
                                     IFileStorage fileStorage)
    {
        _bookRepository = bookRepository;
        _bookPageRepository = bookPageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddBookPageRequest> HandleAsync(AddBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookPage.BookId, command.AccountId, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException();
        }

        var existingBookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, cancellationToken);

        if (existingBookPage == null)
        {
            var bookText = command.BookPage.Text;
            var pageSequenceNumber = command.BookPage.SequenceNumber == 0 ? book.PageCount + 1 : command.BookPage.SequenceNumber;

            var fileName = FilePathHelper.BookPageContentFileName;
            var url = await StoreFile(FilePathHelper.GetBookPageContentPath(command.BookPage.BookId, fileName), bookText, cancellationToken);
            var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);
            command.BookPage.ContentId = file?.Id;

            command.BookPage.Text = string.Empty;
            if (command.BookPage.SequenceNumber == 0) command.BookPage.SequenceNumber = int.MaxValue;
            command.Result = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage, cancellationToken);
            command.Result.Text = bookText;

            command.IsAdded = true;
        }
        else
        {
            command.Result = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, command.BookPage.Text, 0, command.BookPage.Status, command.BookPage.ChapterId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> StoreFile(string filePath, string contents, CancellationToken cancellationToken)
    {
        return await _fileStorage.StoreTextFile(filePath, contents, cancellationToken);
    }

    private async Task<FileModel> AddFile(string fileName, string filePath, string mimeType, CancellationToken cancellationToken)
    {
        return await _fileRepository.AddFile(new FileModel
        {
            FileName = fileName,
            FilePath = filePath,
            MimeType = mimeType,
            DateCreated = DateTime.Now,
            IsPublic = false
        }, cancellationToken);
    }
}
