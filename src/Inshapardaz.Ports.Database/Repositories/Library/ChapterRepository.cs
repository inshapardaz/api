using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.EntityFrameworkCore;
using Chapter = Inshapardaz.Domain.Entities.Library.Chapter;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public ChapterRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Chapter> AddChapter(int bookId, Chapter chapter, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book
                                               .SingleOrDefaultAsync(t => t.Id == bookId,
                                                                     cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            var item = chapter.Map<Chapter, Entities.Library.Chapter>();

            item.Book = book;
            _databaseContext.Chapter.Add(item);

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
                                         .Where(c => c.BookId == bookId)
                                         .OrderBy(c => c.ChapterNumber)
                                         .Select(t => t.Map<Entities.Library.Chapter, Chapter>())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<int> GetChapterCountByBook(int bookId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Chapter
                                         .CountAsync(c => c.BookId == bookId, cancellationToken);
        }

        public async Task<ChapterContent> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            return await _databaseContext.ChapterText
                                         .Include(c => c.Chapter)
                                         .Where(c => c.ChapterId == chapterId)
                                         .Select(t => t.Map<ChapterText, ChapterContent>())
                                         .SingleOrDefaultAsync(cancellationToken);
        }
        
        public async Task<ChapterContent> AddChapterContent(ChapterContent content, CancellationToken cancellationToken)
        {
            var chapterText = new ChapterText
            {
                ChapterId = content.ChapterId,
                Content = content.Content
            };
            _databaseContext.ChapterText
                                         .Add(chapterText);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return chapterText.Map<ChapterText, ChapterContent>();
        }

        public async Task UpdateChapterContent(ChapterContent contents, CancellationToken cancellationToken)
        {
            var content = await _databaseContext.ChapterText.SingleAsync(c => c.ChapterId == contents.ChapterId, cancellationToken);
            content.Content = contents.Content;
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> HasChapterContents(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            return await _databaseContext.ChapterText.AnyAsync(c => c.ChapterId == chapterId, cancellationToken);
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