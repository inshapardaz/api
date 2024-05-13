using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class DeleteChapterContentRequest : BookRequest
{
    public DeleteChapterContentRequest(int libraryId, int bookId, int chapterNumber, string language)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Language = language;
    }

    public int ChapterNumber { get; }

    public string Language { get; }
}

public class DeleteChapterContentRequestHandler : RequestHandlerAsync<DeleteChapterContentRequest>
{
    private readonly IChapterRepository _chapterRepository;

    public DeleteChapterContentRequestHandler(IChapterRepository chapterRepository)
    {
        _chapterRepository = chapterRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await _chapterRepository.DeleteChapterContentById(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
