using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class DeleteChapterContentRequest(int libraryId, int bookId, int chapterNumber, string language)
    : BookRequest(libraryId, bookId)
{
    public int ChapterNumber { get; } = chapterNumber;

    public string Language { get; } = language;
}

public class DeleteChapterContentRequestHandler(
    IChapterRepository chapterRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteChapterContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapterContent = await chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        if (chapterContent != null)
        {
            await commandProcessor.SendAsync(new DeleteTextFileCommand(chapterContent.FileId), cancellationToken: cancellationToken);
            await chapterRepository.DeleteChapterContentById(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
