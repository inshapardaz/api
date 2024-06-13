using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Inshapardaz.Domain.Ports.Command.Library.Book.Page;
using Paramore.Brighter;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookRequest : BookRequest
{
    public DeleteBookRequest(int libraryId, int bookId, int? accountId)
        : base(libraryId, bookId)
    {
        AccountId = accountId;
    }

    public int? AccountId { get; }
}

public class DeleteBookRequestHandler : RequestHandlerAsync<DeleteBookRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteBookRequestHandler(IBookRepository bookRepository,
        IAmACommandProcessor commandProcessor,
        IChapterRepository chapterRepository,
        IBookPageRepository bookPageRepository)
    {
        _bookRepository = bookRepository;
        _commandProcessor = commandProcessor;
        _chapterRepository = chapterRepository;
        _bookPageRepository = bookPageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(book.ImageId), cancellationToken: cancellationToken);
            var chapters = await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
            foreach (var chapter in chapters)
            {
                await _commandProcessor.SendAsync(new DeleteChapterRequest(command.LibraryId, command.BookId, chapter.ChapterNumber), cancellationToken: cancellationToken);
            }

            var pages = await _bookPageRepository.GetAllPagesByBook(command.LibraryId, command.BookId, cancellationToken);
            foreach (var page in pages.OrderByDescending(x => x.SequenceNumber))
            {
                await _commandProcessor.SendAsync(new DeleteBookPageRequest(command.LibraryId, command.BookId, page.SequenceNumber), cancellationToken: cancellationToken);
            }

            var contents = await _bookRepository.GetBookContents(command.LibraryId, command.BookId, cancellationToken);
            foreach (var content in contents)
            {
                await _commandProcessor.SendAsync(new DeleteBookContentRequest(command.LibraryId, command.BookId, content.Id), cancellationToken: cancellationToken);
            }
            await _bookRepository.DeleteBook(command.LibraryId, command.BookId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
