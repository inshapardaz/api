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

        Task<ChapterContent> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken);

        Task<ChapterContent> AddChapterContent(ChapterContent content, CancellationToken cancellationToken);

        Task UpdateChapterContent(ChapterContent contents, CancellationToken cancellationToken);

        Task<bool> HasChapterContents(int bookId, int chapterId, CancellationToken cancellationToken);
    }
}