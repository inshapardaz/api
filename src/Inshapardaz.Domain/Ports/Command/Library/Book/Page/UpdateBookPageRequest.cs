using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateBookPageRequestHandler(IBookRepository bookRepository,
                                     IBookPageRepository bookPageRepository,
                                     IAmACommandProcessor commandProcessor)
    {
        _bookRepository = bookRepository;
        _bookPageRepository = bookPageRepository;
        _commandProcessor = commandProcessor;
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

        var fileName = FilePathHelper.BookPageContentFileName;
        var filePath = FilePathHelper.GetBookPageContentPath(command.BookPage.BookId, fileName);
        var bookText = command.BookPage.Text;

        var saveContentCommand = new SaveTextFileCommand(fileName, filePath, bookText)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = existingBookPage?.ContentId
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.BookPage.ContentId = saveContentCommand.Result.Id;

        if (existingBookPage == null)
        {
            var pageSequenceNumber = command.BookPage.SequenceNumber == 0 ? book.PageCount + 1 : command.BookPage.SequenceNumber;

            command.Result.BookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage, cancellationToken);
            command.Result.BookPage.Text = command.BookPage.Text;
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.BookPage = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, saveContentCommand.Result.Id, existingBookPage.ImageId ?? 0, command.BookPage.Status, command.BookPage.ChapterId, cancellationToken);
            command.Result.BookPage.Text = command.BookPage.Text;
        }

        var previousPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.SequenceNumber - 1, cancellationToken);
        var nextPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.SequenceNumber + 1, cancellationToken);

        command.Result.BookPage.PreviousPage = previousPage;
        command.Result.BookPage.NextPage = nextPage;

        return await base.HandleAsync(command, cancellationToken);
    }
}
