using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChapterByIdQuery(int libraryId, int bookId, int chapterNumber)
    : LibraryBaseQuery<ChapterModel>(libraryId)
{
    public int BookId { get; set; } = bookId;

    public int ChapterNumber { get; } = chapterNumber;
}

public class GetChapterByIdQueryHandler(IChapterRepository chapterRepository, IBookPageRepository bookPageRepository)
    : QueryHandlerAsync<GetChapterByIdQuery, ChapterModel>
{
    public override async Task<ChapterModel> ExecuteAsync(GetChapterByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapter = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        if (chapter != null)
        {
            var firstPage = await bookPageRepository.GetFirstPageIndexByChapterNumber(command.LibraryId, command.BookId, chapter.ChapterNumber, cancellationToken);
            chapter.FirstPageIndex = firstPage?.SequenceNumber;
            chapter.PreviousChapter = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber - 1, cancellationToken);
            chapter.NextChapter = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber + 1, cancellationToken);
        }
        return chapter;
    }
}
