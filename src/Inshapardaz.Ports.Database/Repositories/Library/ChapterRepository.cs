using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;
using Chapter = Inshapardaz.Domain.Entities.Library.Chapter;
using ChapterContent = Inshapardaz.Domain.Entities.Library.ChapterContent;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly IFileStorage _fileStorage;

        public ChapterRepository(IDatabaseContext databaseContext, IFileStorage fileStorage)
        {
            _databaseContext = databaseContext;
            _fileStorage = fileStorage;
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

            var item = chapter.Map();

            item.Book = book;
            _databaseContext.Chapter.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map();
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
            var chapter = await _databaseContext.Chapter
                                                .Include(c => c.Contents)
                                                .SingleOrDefaultAsync(g => g.Id == chapterId, cancellationToken);

            if (chapter != null)
            {
                _databaseContext.Chapter.Remove(chapter);

                foreach (var content in chapter.Contents)
                {
                    if (string.IsNullOrWhiteSpace(content.ContentUrl))
                    {
                        await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                    }
                }

                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Chapter>> GetChaptersByBook(int bookId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Chapter
                                         .Where(c => c.BookId == bookId)
                                         .OrderBy(c => c.ChapterNumber)
                                         .Select(t => t.Map())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<int> GetChapterCountByBook(int bookId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Chapter
                                         .CountAsync(c => c.BookId == bookId, cancellationToken);
        }

        public async Task<IEnumerable<ChapterContent>> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            return await _databaseContext.ChapterContent
                                         .Include(c => c.Chapter)
                                         .ThenInclude(c => c.Book)
                                         .Where(c => c.ChapterId == chapterId)
                                         .Select(t => t.Map())
                                         .ToArrayAsync(cancellationToken);
        }

        public async Task<ChapterContent> GetChapterContentById(int bookId, int chapterId, int id, CancellationToken cancellationToken)
        {
            var chapterContent = await _databaseContext.ChapterContent
                                                       .Include(c => c.Chapter)
                                                       .ThenInclude(c => c.Book)
                                                       .Where(c => c.Id == id)
                                                       .SingleOrDefaultAsync(cancellationToken);

            if (chapterContent != null && !string.IsNullOrWhiteSpace(chapterContent.ContentUrl))
            {
                var result = chapterContent.Map();
                var content = await _fileStorage.GetTextFile(chapterContent.ContentUrl, cancellationToken);
                if (content != null)
                {
                    result.Content = content;
                    return result;
                }
            }
            
            return null;
        }

        public async Task<ChapterContent> GetChapterContent(int bookId, int chapterId, string mimeType, CancellationToken cancellationToken)
        {
            var chapterContent = await _databaseContext.ChapterContent
                                                       .Include(c => c.Chapter)
                                                       .ThenInclude(c => c.Book)
                                                       .Where(c => c.ChapterId == chapterId && c.MimeType == mimeType)
                                                       .SingleOrDefaultAsync(cancellationToken);

            if (chapterContent != null && !string.IsNullOrWhiteSpace(chapterContent.ContentUrl))
            {
                var result = chapterContent.Map();
                var content = await _fileStorage.GetTextFile(chapterContent.ContentUrl, cancellationToken);
                if (content != null)
                {
                    result.Content = content;
                    return result;
                }
            }
            
            return null;
        }

        public async Task<ChapterContent> AddChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken)
        {
            var name = GenerateChapterContentUrl(bookId, chapterId, mimeType);
            var actualUrl = await _fileStorage.StoreTextFile(name, contents, cancellationToken);
            var chapterContent = new Entities.Library.ChapterContent
            {
                ChapterId = chapterId,
                MimeType = mimeType,
                ContentUrl = actualUrl
            };
            _databaseContext.ChapterContent
                            .Add(chapterContent);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            var result = chapterContent.Map();
            result.Content = contents;
            return result;
        }

        public async Task UpdateChapterContent(int bookId, int chapterId, string mimeType, string contents, CancellationToken cancellationToken)
        {
            var content = await _databaseContext.ChapterContent.SingleAsync(c => c.ChapterId == chapterId && c.MimeType == mimeType, cancellationToken);

            string name = content != null ? content.ContentUrl : GenerateChapterContentUrl(bookId, chapterId, mimeType);
            var actualUrl = await _fileStorage.StoreTextFile(name, contents, cancellationToken);

            if (content == null)
            {
                var entity = new Entities.Library.ChapterContent()
                {
                    ChapterId = chapterId,
                    MimeType = mimeType,
                    ContentUrl = actualUrl
                };

                await _databaseContext.ChapterContent.AddAsync(entity, cancellationToken);
            }
            else if (content.ContentUrl != actualUrl)
            {
                content.ContentUrl = actualUrl;
                await _databaseContext.SaveChangesAsync(cancellationToken);

            }
        }

        public async Task DeleteChapterContentById(int bookId, int chapterId, int contentId, CancellationToken cancellationToken)
        {
            var content = await _databaseContext.ChapterContent.SingleOrDefaultAsync(c => c.Id == contentId, cancellationToken);
            if (content != null)
            {
                await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                _databaseContext.ChapterContent.Remove(content);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Chapter> GetChapterById(int chapterId, CancellationToken cancellationToken)
        {
            var chapter = await _databaseContext.Chapter
                                                .Include(c => c.Contents)
                                               .SingleOrDefaultAsync(t => t.Id == chapterId,
                                                                     cancellationToken);
            return chapter.Map();
        }

        private string GenerateChapterContentUrl(int bookId, int chapterId, string mimeType)
        {
            var extension = MimetypeToExtension(mimeType);
            return $"books/{bookId}/chapters/{chapterId}.{extension}";
        }

        private string MimetypeToExtension(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "text/plain": return "txt";
                case "text/markdown": return "md";
                case "text/html": return "md";
                case "application/msword": return "doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "doc";
                case "application/pdf": return "pdf";
                case "application/epub+zip": return "epub";
                default: throw new BadRequestException();
            }
        }
    }
}
