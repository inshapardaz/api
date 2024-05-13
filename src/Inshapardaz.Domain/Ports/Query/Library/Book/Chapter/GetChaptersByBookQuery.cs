using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChaptersByBookQuery : LibraryBaseQuery<IEnumerable<ChapterModel>>
{
    public GetChaptersByBookQuery(int libraryId, int bookId)
        : base(libraryId)
    {
        BookId = bookId;
    }

    public int BookId { get; set; }
}

public class GetChaptersByBookQuerytHandler : QueryHandlerAsync<GetChaptersByBookQuery, IEnumerable<ChapterModel>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IUserHelper _userHelper;

    public GetChaptersByBookQuerytHandler(IBookRepository bookRepository, IChapterRepository chapterRepository, IUserHelper userHelper)
    {
        _bookRepository = bookRepository;
        _chapterRepository = chapterRepository;
        _userHelper = userHelper;
    }

    public override async Task<IEnumerable<ChapterModel>> ExecuteAsync(GetChaptersByBookQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, _userHelper.AccountId, cancellationToken);
        if (book == null) return null;

        return await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
    }
}
