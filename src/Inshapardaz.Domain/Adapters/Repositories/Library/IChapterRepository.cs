using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IChapterRepository
    {
        Task<ChapterModel> AddChapter(int bookId, ChapterModel chapter, CancellationToken cancellationToken);

        Task UpdateChapter(ChapterModel chapter, CancellationToken cancellationToken);

        Task DeleteChapter(int chapterId, CancellationToken cancellationToken);

        Task<IEnumerable<ChapterModel>> GetChaptersByBook(int bookId, CancellationToken cancellationToken);
        
        Task<ChapterModel> GetChapterById(int chapterId, CancellationToken cancellationToken);

        Task<int> GetChapterCountByBook(int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<ChapterContentModel>> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken);

        Task<ChapterContentModel> GetChapterContentById(int bookId, int chapterId, int id, CancellationToken cancellationToken);
        
        Task<ChapterContentModel> GetChapterContent(int bookId, int chapterId, string mimeType, CancellationToken cancellationToken);

        Task<ChapterContentModel> AddChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken);

        Task UpdateChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken);

        Task DeleteChapterContentById(int bookId, int chapterId, int contentId, CancellationToken cancellationToken);
    }
}
