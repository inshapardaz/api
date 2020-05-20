using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IChapterRepository
    {
        Task<ChapterModel> AddChapter(int libraryId, int bookId, ChapterModel chapter, CancellationToken cancellationToken);

        Task UpdateChapter(int libraryId, int bookId, ChapterModel chapter, CancellationToken cancellationToken);

        Task DeleteChapter(int libraryId, int bookId, int chapterId, CancellationToken cancellationToken);

        Task<IEnumerable<ChapterModel>> GetChaptersByBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task<ChapterModel> GetChapterById(int libraryId, int bookid, int chapterId, CancellationToken cancellationToken);

        Task<ChapterContentModel> GetChapterContent(int libraryId, int bookId, int chapterId, string language, string mimeType, CancellationToken cancellationToken);

        Task<string> GetChapterContentUrl(int libraryId, int bookId, int chapterId, string language, string mimeType, CancellationToken cancellationToken);

        Task<ChapterContentModel> AddChapterContent(int libraryId, ChapterContentModel content, CancellationToken cancellationToken);

        Task UpdateChapterContent(int libraryId, int bookId, int chapterId, string language, string mimeType, string contents, CancellationToken cancellationToken);

        Task DeleteChapterContentById(int libraryId, int bookId, int chapterId, string language, string mimeType, CancellationToken cancellationToken);
    }
}
