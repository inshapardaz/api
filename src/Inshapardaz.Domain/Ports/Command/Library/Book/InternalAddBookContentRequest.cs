using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class InternalAddBookContentRequest(int libraryId, int bookId, string language, string mimeType, int? accountId)
    : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;

    public string Language { get; } = language;
    public string MimeType { get; } = mimeType;
    public int? AccountId { get; } = accountId;
    public FileModel Content { get; set; }

    public BookContentModel Result { get; set; }
}

public class InternalAddBookContentRequestHandler(IBookRepository bookRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<InternalAddBookContentRequest>
{
    public override async Task<InternalAddBookContentRequest> HandleAsync(InternalAddBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);

        if (book != null)
        {
            var status = (await bookRepository.GetBookPageSummary(command.LibraryId, new[] { book.Id }, cancellationToken)).FirstOrDefault();

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

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, FilePathHelper.GetBookContentPath(command.BookId, fileName), command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                IsPublic = command.Content.IsPublic
            };

            await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            var contentId = await bookRepository.AddBookContent(book.Id, saveFileCommand.Result.Id, command.Language, cancellationToken);

            command.Result = await bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
