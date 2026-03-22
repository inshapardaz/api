using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AddChapterRequest(int libraryId, int bookId, ChapterModel chapter) : BookRequest(libraryId, bookId)
{
    public ChapterModel Chapter { get; } = chapter;

    public ChapterModel Result { get; set; }
}

public class AddChapterRequestHandler(
    IChapterRepository chapterRepository,
    IBookRepository bookRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<AddChapterRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddChapterRequest> HandleAsync(AddChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, userHelper.AccountId, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException();
        }

        command.Chapter.ChapterNumber = book.ChapterCount + 1;
        command.Result = await chapterRepository.AddChapter(command.LibraryId, command.BookId, command.Chapter, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
