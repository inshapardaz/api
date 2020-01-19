using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IChapterRepository
    {
        Task<Chapter> AddChapter(int bookId, Chapter chapter, CancellationToken cancellationToken);

        Task UpdateChapter(Chapter chapter, CancellationToken cancellationToken);

        Task DeleteChapter(int chapterId, CancellationToken cancellationToken);

        Task<IEnumerable<Chapter>> GetChaptersByBook(int bookId, CancellationToken cancellationToken);
        
        Task<Chapter> GetChapterById(int chapterId, CancellationToken cancellationToken);

        Task<int> GetChapterCountByBook(int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<ChapterContent>> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken);

        Task<ChapterContent> GetChapterContentById(int bookId, int chapterId, int id, CancellationToken cancellationToken);
        
        Task<ChapterContent> GetChapterContent(int bookId, int chapterId, string mimeType, CancellationToken cancellationToken);

        Task<ChapterContent> AddChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken);

        Task UpdateChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken);

        Task DeleteChapterContentById(int bookId, int chapterId, int contentId, CancellationToken cancellationToken);
    }
}
