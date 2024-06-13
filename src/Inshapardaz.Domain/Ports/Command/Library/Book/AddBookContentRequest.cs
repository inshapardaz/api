using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public AddBookFileRequestHandler(IBookRepository bookRepository,  IAmACommandProcessor commandProcessor)
    {
        _bookRepository = bookRepository;
        _commandProcessor = commandProcessor;
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

            var fileName = FilePathHelper.GetBookContentFileName(command.Content.FileName);

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, FilePathHelper.GetBookContentPath(command.LibraryId, command.BookId, fileName), command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                IsPublic = command.Content.IsPublic
            };

            await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            var contentId = await _bookRepository.AddBookContent(book.Id, saveFileCommand.Result.Id, command.Language, cancellationToken);

            command.Result = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
