using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IChapterRepository
    {
        Task<ChapterModel> AddChapter(int bookId, ChapterModel chapter, CancellationToken cancellationToken);

        Task UpdateChapter(ChapterModel chapter, CancellationToken cancellationToken);

        Task DeleteChapter(int chapterId, CancellationToken cancellationToken);

        Task<IEnumerable<ChapterModel>> GetChaptersByBook(int bookId, CancellationToken cancellationToken);

        Task<ChapterModel> GetChapterById(int chapterId, CancellationToken cancellationToken);

        Task<ChapterContentModel> GetChapterContent(int chapterId, string mimeType, CancellationToken cancellationToken);

        Task<string> GetChapterContentUrl(int chapterId, string mimeType, CancellationToken cancellationToken);

        Task<ChapterContentModel> AddChapterContent(int bookId, int chapterId, string mimeType, string contentUrl, CancellationToken cancellationToken);

        Task UpdateChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken);

        Task DeleteChapterContentById(int bookId, int chapterId, string mimeType, CancellationToken cancellationToken);
    }
}
