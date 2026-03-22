using Inshapardaz.Domain.Exception;
using Paramore.Darker;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChapterContentQuery(int libraryId, int bookId, int chapterNumber, string language)
    : LibraryBaseQuery<ChapterContentModel>(libraryId)
{
    public int BookId { get; set; } = bookId;

    public int ChapterNumber { get; } = chapterNumber;

    public string Language { get; set; } = language;
}

public class GetChapterContentQueryHandler(
    ILibraryRepository libraryRepository,
    IBookRepository bookRepository,
    IChapterRepository chapterRepository,
    IUserHelper userHelper,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : QueryHandlerAsync<GetChapterContentQuery, ChapterContentModel>
{
    [LibraryAuthorize(1)]
    public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException();
        }

        if (!book.IsPublic && !userHelper.AccountId.HasValue)
        {
            throw new UnauthorizedException();
        }

        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var chapterContent = await chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        if (chapterContent != null)
        {
            if (chapterContent.FileId.HasValue)
            {
                var file = await fileRepository.GetFileById(chapterContent.FileId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    chapterContent.Text = fc;
                }
            }
            if (userHelper.AccountId.HasValue)
            {
                await bookRepository.AddRecentBook(
                    command.LibraryId, 
                    userHelper.AccountId.Value, 
                    command.BookId, new ReadProgressModel()
                    {
                        ProgressType = ProgressType.Chapter,
                        ProgressId = command.ChapterNumber,
                        ProgressValue = 0.0
                    }, 
                    cancellationToken);
            }
        }

        return chapterContent;
    }
}
