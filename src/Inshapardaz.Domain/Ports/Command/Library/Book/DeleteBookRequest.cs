using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Inshapardaz.Domain.Ports.Command.Library.Book.Page;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookRequest(int libraryId, int bookId, int? accountId) : BookRequest(libraryId, bookId)
{
    public int? AccountId { get; } = accountId;
}

public class DeleteBookRequestHandler(
    IBookRepository bookRepository,
    IAmACommandProcessor commandProcessor,
    IChapterRepository chapterRepository,
    IBookPageRepository bookPageRepository)
    : RequestHandlerAsync<DeleteBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(book.ImageId), cancellationToken: cancellationToken);
            var chapters = await chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
            foreach (var chapter in chapters)
            {
                await commandProcessor.SendAsync(new DeleteChapterRequest(command.LibraryId, command.BookId, chapter.ChapterNumber), cancellationToken: cancellationToken);
            }

            var pages = await bookPageRepository.GetAllPagesByBook(command.LibraryId, command.BookId, cancellationToken);
            foreach (var page in pages.OrderByDescending(x => x.SequenceNumber))
            {
                await commandProcessor.SendAsync(new DeleteBookPageRequest(command.LibraryId, command.BookId, page.SequenceNumber), cancellationToken: cancellationToken);
            }

            var contents = await bookRepository.GetBookContents(command.LibraryId, command.BookId, cancellationToken);
            foreach (var content in contents)
            {
                await commandProcessor.SendAsync(new DeleteBookContentRequest(command.LibraryId, command.BookId, content.Id), cancellationToken: cancellationToken);
            }
            await bookRepository.DeleteBook(command.LibraryId, command.BookId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
