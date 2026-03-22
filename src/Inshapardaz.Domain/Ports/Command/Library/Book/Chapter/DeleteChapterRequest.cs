using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class DeleteChapterRequest(int libraryId, int bookId, int chapterNumber) : BookRequest(libraryId, bookId)
{
    public int ChapterNumber { get; } = chapterNumber;
}

public class DeleteChapterRequestHandler(
    IChapterRepository chapterRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteChapterRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapterContents = await chapterRepository.GetChapterContents(command.LibraryId,
                    command.BookId, command.ChapterNumber, cancellationToken);
        if (chapterContents != null)
        {
            foreach (var chapterContent in chapterContents)
            {
                await commandProcessor.SendAsync(new DeleteTextFileCommand(chapterContent.FileId), cancellationToken: cancellationToken);
            }
        }

        await chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
