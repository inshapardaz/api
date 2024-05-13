using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AddChapterContentRequest : BookRequest
{
    public AddChapterContentRequest(int libraryId, int bookId, int chapterNumber, string contents, string language)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Contents = contents;
        Language = language;
    }

    public int ChapterNumber { get; set; }

    public string Contents { get; }

    public string Language { get; set; }

    public ChapterContentModel Result { get; set; }
}

public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILibraryRepository _libraryRepository;

    public AddChapterContentRequestHandler(IChapterRepository chapterRepository, ILibraryRepository libraryRepository)
    {
        _chapterRepository = chapterRepository;
        _libraryRepository = libraryRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter != null)
        {
            var chapterContent = new ChapterContentModel
            {
                BookId = command.BookId,
                ChapterId = chapter.Id,
                ChapterNumber = chapter.ChapterNumber,
                Language = command.Language,
                Text = command.Contents
            };

            command.Result = await _chapterRepository.AddChapterContent(command.LibraryId, chapterContent, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
