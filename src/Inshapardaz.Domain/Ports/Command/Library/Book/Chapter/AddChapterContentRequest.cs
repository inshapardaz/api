using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AddChapterContentRequest(int libraryId, int bookId, int chapterNumber, string contents, string language)
    : BookRequest(libraryId, bookId)
{
    public int ChapterNumber { get; set; } = chapterNumber;

    public string Contents { get; } = contents;

    public string Language { get; set; } = language;

    public ChapterContentModel Result { get; set; }
}

public class AddChapterContentRequestHandler(
    ILibraryRepository libraryRepository,
    IChapterRepository chapterRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<AddChapterContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var chapter = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter != null)
        {
            var fileName = FilePathHelper.BookChapterContentFileName;

            var saveFileCommand = new SaveTextFileCommand(fileName, FilePathHelper.GetBookChapterContentPath(command.BookId, fileName), command.Contents)
            {
                MimeType = MimeTypes.Markdown
            };

            await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            var chapterContent = new ChapterContentModel
            {
                BookId = command.BookId,
                ChapterId = chapter.Id,
                ChapterNumber = chapter.ChapterNumber,
                Language = command.Language,
                FileId = saveFileCommand.Result?.Id
            };

            command.Result = await chapterRepository.AddChapterContent(command.LibraryId, chapterContent, cancellationToken);
            command.Result.Text = command.Contents;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
