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

public class UpdateBookPageRequest : LibraryBaseCommand
{
    public UpdateBookPageRequest(int libraryId, int bookId, int sequenceNumber, int? accountId, BookPageModel book)
    : base(libraryId)
    {
        BookPage = book;
        BookPage.BookId = bookId;
        SequenceNumber = sequenceNumber;
        AccountId = accountId;
    }

    public BookPageModel BookPage { get; }

    public RequestResult Result { get; set; } = new RequestResult();
    public int SequenceNumber { get; set; }
    public int? AccountId { get; }

    public class RequestResult
    {
        public BookPageModel BookPage { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookPageRequestHandler : RequestHandlerAsync<UpdateBookPageRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    public UpdateBookPageRequestHandler(IBookRepository bookRepository,
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
    public override async Task<UpdateBookPageRequest> HandleAsync(UpdateBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookPage.BookId, command.AccountId, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException();
        }

        var existingBookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, cancellationToken);

        if (existingBookPage == null)
        {
            var pageSequenceNumber = command.BookPage.SequenceNumber == 0 ? book.PageCount + 1 : command.BookPage.SequenceNumber;

            var fileName = FilePathHelper.BookPageContentFileName;
            var url = await StoreFile(FilePathHelper.GetBookPageContentPath(command.BookPage.BookId, fileName), command.BookPage.Text, cancellationToken);
            var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);
            command.BookPage.ContentId = file?.Id;

            command.Result.BookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage, cancellationToken);
            command.Result.BookPage.Text = command.BookPage.Text;
            command.Result.HasAddedNew = true;
        }
        else
        {
            long fileId;

            if (existingBookPage.ContentId.HasValue)
            {
                var file = await _fileRepository.GetFileById(existingBookPage.ContentId.Value, cancellationToken);
                await _fileStorage.TryDeleteFile(file.FilePath, cancellationToken);
                fileId = file.Id;
            }
            else
            {
                var fileName = FilePathHelper.BookPageContentFileName;
                var url = await StoreFile(FilePathHelper.GetBookPageContentPath(command.BookPage.BookId, fileName), command.BookPage.Text, cancellationToken);
                var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);
                fileId = file.Id;
            }

            command.Result.BookPage = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, command.BookPage.Text, existingBookPage.ImageId ?? 0, command.BookPage.Status, command.BookPage.ChapterId, cancellationToken);
            command.Result.BookPage.Text = command.BookPage.Text;
        }

        var previousPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.SequenceNumber - 1, cancellationToken);
        var nextPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.SequenceNumber + 1, cancellationToken);

        command.Result.BookPage.PreviousPage = previousPage;
        command.Result.BookPage.NextPage = nextPage;

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
