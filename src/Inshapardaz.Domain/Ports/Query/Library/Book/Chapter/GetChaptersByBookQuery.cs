using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChaptersByBookQuery(int libraryId, int bookId) : LibraryBaseQuery<IEnumerable<ChapterModel>>(libraryId)
{
    public int BookId { get; set; } = bookId;
}

public class GetChaptersByBookQuerytHandler(
    IBookRepository bookRepository,
    IChapterRepository chapterRepository,
    IUserHelper userHelper,
    IBookPageRepository bookPageRepository)
    : QueryHandlerAsync<GetChaptersByBookQuery, IEnumerable<ChapterModel>>
{
    public override async Task<IEnumerable<ChapterModel>> ExecuteAsync(GetChaptersByBookQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, userHelper.AccountId, cancellationToken);
        if (book == null) return null;

        var chapters = await chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
        foreach (var chapter in chapters)
        {
            var firstPage = await bookPageRepository.GetFirstPageIndexByChapterNumber(command.LibraryId, command.BookId, chapter.ChapterNumber, cancellationToken);
            chapter.FirstPageIndex = firstPage?.SequenceNumber;
        }
        return chapters;
    }
}
