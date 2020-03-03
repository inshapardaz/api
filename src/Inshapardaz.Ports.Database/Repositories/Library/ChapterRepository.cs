using Dapper;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public ChapterRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<ChapterModel> AddChapter(int bookId, ChapterModel chapter, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Library.Chapter (Title, BookId, ChapterNumber) Output Inserted.Id Values (@Title, @BookId, @ChapterNumber)";
                var command = new CommandDefinition(sql, new { Title = chapter.Title, BookId = bookId, ChapterNumber = chapter.ChapterNumber }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetChapterById(id, cancellationToken);
        }

        public async Task UpdateChapter(ChapterModel chapter, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Update Library.Chapter Set Title = @Title, BookId = @BookId, ChapterNumber = @ChapterNumber Where Id = @Id";
                var command = new CommandDefinition(sql, chapter, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteChapter(int chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Delete From Library.Chapter Where Id = @Id";
                var command = new CommandDefinition(sql, new { Id = chapterId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<ChapterModel>> GetChaptersByBook(int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var chapters = new Dictionary<int, ChapterModel>();

                var sql = @"Select c.*, cc.Id, cc.MimeType From Library.Chapter c
                            Left Outer Join Library.ChapterContent cc On c.Id = cc.ChapterId
                            Where c.BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ChapterModel, int, string, ChapterModel>(command, (c, id, mimeType) =>
                {
                    if (!chapters.TryGetValue(c.Id, out ChapterModel chapter))
                        chapters.Add(c.Id, chapter = c);

                    chapter.Contents.Add(new ChapterContentModel
                    {
                        BookId = c.BookId,
                        ChapterId = c.Id,
                        MimeType = mimeType,
                        Id = id
                    });

                    return chapter;
                });

                return chapters.Values;
            }
        }

        public async Task<ChapterModel> GetChapterById(int chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ChapterModel chapter = null;
                var sql = @"Select c.*, cc.Id, cc.MimeType From Library.Chapter c
                            Left Outer Join Library.ChapterContent cc On c.Id = cc.ChapterId
                            Where Id = @Id";
                var command = new CommandDefinition(sql, new { Id = chapterId }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ChapterModel, int, string, ChapterModel>(command, (c, id, mimeType) =>
                {
                    if (chapter == null)
                    {
                        chapter = c;
                    }

                    chapter.Contents.Add(new ChapterContentModel
                    {
                        BookId = c.BookId,
                        ChapterId = c.Id,
                        MimeType = mimeType,
                        Id = id
                    });

                    return chapter;
                });

                return chapter;
            }
        }

        public async Task<ChapterContentModel> GetChapterContent(int chapterId, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select cc.* From Library.Chapter c
                            Left Outer Join Library.ChapterContent cc On c.Id = cc.ChapterId
                            Where c.Id = @Id And cc.MimeType = @MimeType";
                var command = new CommandDefinition(sql, new { Id = chapterId, MimeType = mimeType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ChapterContentModel>(command);
            }
        }

        public async Task<string> GetChapterContentUrl(int chapterId, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select cc.ContentUrl From Library.Chapter c
                            Left Outer Join Library.ChapterContent cc On c.Id = cc.ChapterId
                            Where c.Id = @Id And cc.MimeType = @MimeType";
                var command = new CommandDefinition(sql, new { Id = chapterId, MimeType = mimeType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<string>(command);
            }
        }

        public async Task<ChapterContentModel> AddChapterContent(int bookId, int chapterId, string mimeType, string contentUrl, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Library.ChapterContent (ContentUrl, ChapterId, MimeType) Values (@ContentUrl, @ChapterId, @MimeType)";
                var command = new CommandDefinition(sql, new { ChapterId = chapterId, MimeType = mimeType, ContentUrl = contentUrl }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetChapterContent(chapterId, mimeType, cancellationToken);
            }
        }

        public async Task UpdateChapterContent(int bookId, int chapterId, string mimeType, string contentUrl, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.ChapterContent SET ContentUrl = @ContentUrl Where ChapterId = @ChapterId And MimeType @MimeType";
                var command = new CommandDefinition(sql, new { ChapterId = chapterId, MimeType = mimeType, ContentUrl = contentUrl }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteChapterContentById(int bookId, int chapterId, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.ChapterContent Where ChapterId = @ChapterId And MimeType @MimeType";
                var command = new CommandDefinition(sql, new { ChapterId = chapterId, MimeType = mimeType }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
