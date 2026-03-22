using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class UpdateChapterRequest : BookRequest
{
    public UpdateChapterRequest(int libraryId, int bookId, int chapterNumber, ChapterModel chapter)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Chapter = chapter;
        Chapter.BookId = bookId;
    }

    public int ChapterNumber { get; }
    public ChapterModel Chapter { get; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public ChapterModel Chapter { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateChapterRequestHandler(IChapterRepository chapterRepository)
    : RequestHandlerAsync<UpdateChapterRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateChapterRequest> HandleAsync(UpdateChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        if (result == null)
        {
            var chapter = command.Chapter;
            chapter.Id = default;
            command.Result.Chapter = await chapterRepository.AddChapter(command.LibraryId, command.BookId, chapter, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Chapter.Id = result.Id;
            command.Result.Chapter = await chapterRepository.UpdateChapter(command.LibraryId, command.BookId, command.ChapterNumber, command.Chapter, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
