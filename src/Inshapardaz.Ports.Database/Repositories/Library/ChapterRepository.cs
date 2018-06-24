using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public ChapterRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Chapter> AddChapter(Chapter chapter, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book
                                               .SingleOrDefaultAsync(t => t.Id == chapter.BookId,
                                                                     cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            var item = chapter.Map<Chapter, Entities.Library.Chapter>();
            book.Chapters.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Chapter, Chapter>();
        }

        public async Task UpdateChapter(Chapter chapter, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Chapter
                                                       .SingleOrDefaultAsync(g => g.Id == chapter.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Title = chapter.Title;
            existingEntity.ChapterNumber = chapter.ChapterNumber;
            existingEntity.BookId = chapter.BookId;
            existingEntity.Content.Content = chapter.Content;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteChapter(int chapterId, CancellationToken cancellationToken)
        {
            var chapter = await _databaseContext.Chapter.SingleOrDefaultAsync(g => g.Id == chapterId, cancellationToken);

            if (chapter == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Chapter.Remove(chapter);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Chapter>> GetChaptersByBook(int bookId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Chapter
                                         .Select(t => t.Map<Entities.Library.Chapter, Chapter>())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<Chapter> GetChapterById(int chapterId, CancellationToken cancellationToken)
        {
            var chapter = await _databaseContext.Chapter
                                                .Include(c => c.Content)
                                               .SingleOrDefaultAsync(t => t.Id == chapterId,
                                                                     cancellationToken);
            return chapter.Map<Entities.Library.Chapter, Chapter>();
        }
    }
}