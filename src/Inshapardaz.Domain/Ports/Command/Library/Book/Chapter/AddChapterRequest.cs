using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AddChapterRequest : BookRequest
{
    public AddChapterRequest(int libraryId, int bookId, ChapterModel chapter)
        : base(libraryId, bookId)
    {
        Chapter = chapter;
    }

    public ChapterModel Chapter { get; }

    public ChapterModel Result { get; set; }
}

public class AddChapterRequestHandler : RequestHandlerAsync<AddChapterRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserHelper _userHelper;

    public AddChapterRequestHandler(IChapterRepository chapterRepository, IBookRepository bookRepository, IUserHelper userHelper)
    {
        _chapterRepository = chapterRepository;
        _bookRepository = bookRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddChapterRequest> HandleAsync(AddChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, _userHelper.AccountId, cancellationToken);
        if (book == null)
        {
            throw new BadRequestException();
        }

        command.Chapter.ChapterNumber = book.ChapterCount + 1;
        command.Result = await _chapterRepository.AddChapter(command.LibraryId, command.BookId, command.Chapter, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
