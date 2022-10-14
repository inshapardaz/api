using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public ChapterRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<ChapterModel> AddChapter(int libraryId, int bookId, ChapterModel chapter, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Chapter (Title, BookId, ChapterNumber) Output Inserted.Id Values (@Title, @BookId, @ChapterNumber)";
                var command = new CommandDefinition(sql, new { Title = chapter.Title, BookId = bookId, ChapterNumber = chapter.ChapterNumber }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            await ReorderChapters(libraryId, bookId, cancellationToken);
            return  await GetChapterByChapterId(id, cancellationToken);
        }

        public async Task UpdateChapter(int libraryId, int bookId, int oldChapterNumber, ChapterModel chapter, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update C Set C.Title = @Title, C.BookId = @BookId, C.ChapterNumber = @ChapterNumber, C.Status = @Status
                            From Chapter C
                            Inner Join Book b On b.Id = C.BookId
                            Where C.chapterNumber = @OldChapterNumber AND C.BookId = @OldBookId And b.LibraryId = @LibraryId";
                var args = new
                {
                    LibraryId = libraryId,
                    OldBookId = bookId,
                    ChapterId = chapter.Id,
                    Title = chapter.Title,
                    Status = chapter.Status,
                    BookId = chapter.BookId,
                    OldChapterNumber = oldChapterNumber,
                    ChapterNumber = chapter.ChapterNumber
                };
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }

            await ReorderChapters(libraryId, bookId, cancellationToken);
        }

        public async Task UpdateChaptersSequence(int libraryId, int bookId, IEnumerable<ChapterModel> chapters, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update C Set C.ChapterNumber = @ChapterNumber
                            From Chapter C
                            Inner Join Book b On b.Id = C.BookId
                            Where C.Id = @Id AND C.BookId = @BookId And b.LibraryId = @LibraryId";
                var args = chapters.Select(c => new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    Id = c.Id,
                    ChapterNumber = c.ChapterNumber
                });
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteChapter(int libraryId, int bookId, int chapterNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete c From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Where c.chapterNumber = @ChapterId AND c.BookId = @BookId And b.LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    ChapterId = chapterNumber
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }

            await ReorderChapters(libraryId, bookId, cancellationToken);
        }

        public async Task<IEnumerable<ChapterModel>> GetChaptersByBook(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var chapters = new Dictionary<int, ChapterModel>();

                var sql = @"Select c.*, cc.Id as ContentId, cc.Language As ContentLanguage,
                            a.Name As WriterAccountName, ar.Name As ReviewerAccountName
                            From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Left Outer Join ChapterContent cc On c.Id = cc.ChapterId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = c.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = c.ReviewerAccountId
                            Where b.Id = @BookId AND b.LibraryId = @LibraryId
                            Order By c.ChapterNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ChapterModel, int?, string, string, string, ChapterModel>(command, (c, contentId, contentLangugage, WriterAccountName, ReviewerAccountName) =>
                {
                    if (!chapters.TryGetValue(c.Id, out ChapterModel chapter))
                    {
                        c.WriterAccountName = WriterAccountName;
                        c.ReviewerAccountName = ReviewerAccountName;

                        chapters.Add(c.Id, chapter = c);
                    }

                    chapter = chapters[c.Id];
                    if (contentId != null)
                    {
                        var content = chapter.Contents.SingleOrDefault(x => x.Id == contentId);
                        if (content == null)
                        {
                            ChapterContentModel cc = new ChapterContentModel();
                            cc.BookId = bookId;
                            cc.ChapterId = c.Id;
                            cc.ChapterNumber = c.ChapterNumber;
                            cc.Id = contentId.Value;
                            cc.Language = contentLangugage;
                            chapter.Contents.Add(cc);
                        }
                    }

                    return chapter;
                }, splitOn: "ContentId,ContentLanguage,WriterAccountName,ReviewerAccountName");

                return chapters.Values;
            }
        }

        public async Task<ChapterModel> GetChapterById(int libraryId, int bookId, int chapterNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ChapterModel chapter = null;
                var sql = @"Select c.*, cc.*, a.*, ar.*
                            From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Left Outer Join ChapterContent cc On c.Id = cc.ChapterId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = c.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = c.ReviewerAccountId
                            Where c.chapterNumber = @ChapterNumber AND b.Id = @BookId AND b.LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, ChapterNumber = chapterNumber }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ChapterModel, ChapterContentModel, AccountModel, AccountModel, ChapterModel>(command, (c, cc, wa, ra) =>
                {
                    if (chapter == null)
                    {
                        chapter = c;
                        chapter.WriterAccountName = wa?.Name;
                        chapter.ReviewerAccountName = ra?.Name;
                    }

                    if (cc != null)
                    {
                        var content = chapter.Contents.SingleOrDefault(x => x.Id == cc.Id);
                        if (content == null)
                        {
                            cc.ChapterNumber = c.ChapterNumber;
                            cc.BookId = c.BookId;
                            chapter.Contents.Add(cc);
                        }
                    }

                    return chapter;
                });

                return chapter;
            }
        }

        public async Task<ChapterContentModel> GetChapterContent(int libraryId, int bookId, int chapterNumber, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select cc.*, c.chapterNumber, b.Id As BookId From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Left Outer Join ChapterContent cc On c.Id = cc.ChapterId
                            Where c.chapterNumber = @ChapterId AND b.Id = @BookId AND b.LibraryId = @LibraryId AND cc.Language = @Language";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, ChapterId = chapterNumber, Language = language }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ChapterContentModel>(command);
            }
        }

        public async Task<ChapterContentModel> AddChapterContent(int libraryId, ChapterContentModel content, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into ChapterContent (ChapterId, Language, Text)
                            Values (@ChapterId, @Language, @Text)";
                var command = new CommandDefinition(sql, new { ChapterId = content.ChapterId, Language = content.Language, Text = content.Text }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetChapterContent(libraryId, content.BookId, content.ChapterNumber, content.Language, cancellationToken);
            }
        }

        public async Task UpdateChapterContent(int libraryId, int bookId, int chapterNumber, string language, string text, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update cc SET Text = @Text
                            FROM ChapterContent cc
                            Inner Join Chapter c On c.Id = cc.ChapterId
                            Inner Join Book b On b.Id = c.BookId
                            Where c.ChapterNumber = @ChapterNumber And b.LibraryId = @LibraryId and b.Id = @BookId AND cc.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    ChapterNumber = chapterNumber,
                    Language = language,
                    Text = text
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteChapterContentById(int libraryId, int bookId, int chapterNumber, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete cc
                            From ChapterContent cc
                            Inner Join Chapter c On c.Id = cc.ChapterId
                            Inner Join Book b On b.Id = C.BookId
                            Where c.chapterNumber = @ChapterNumber And b.LibraryId = @LibraryId and b.Id = @BookId AND cc.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    ChapterNumber = chapterNumber,
                    Language = language
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task ReorderChapters(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT c.Id, row_number() OVER (ORDER BY c.ChapterNumber) as 'ChapterNumber'
                            From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Where c.BookId = @BookId And b.LibraryId = @LibraryId
                            Order By c.ChapterNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId
                }, cancellationToken: cancellationToken);
                var newOrder = await connection.QueryAsync(command);

                var sql2 = @"UPDATE Chapter
                            SET ChapterNumber = @ChapterNumber
                            Where Id = @Id";
                var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task<ChapterModel> UpdateWriterAssignment(int libraryId, int bookId, int chapterNumber, int? assignedAccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update c
                            SET c.WriterAccountId = @WriterAccountId, c.WriterAssignTimeStamp = GETUTCDATE()
                            FROM Chapter c
                            INNER JOIN Book b ON b.Id = c.BookId
                            Where b.LibraryId = @LibraryId AND c.BookId = @BookId AND c.ChapterNumber = @ChapterNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, WriterAccountId = assignedAccountId, BookId = bookId, ChapterNumber = chapterNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetChapterById(libraryId, bookId, chapterNumber, cancellationToken);
            }
        }

        public async Task<ChapterModel> UpdateReviewerAssignment(int libraryId, int bookId, int chapterNumber, int? assignedAccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update c
                            SET c.ReviewerAccountId = @ReviewerAccountId, c.ReviewerAssignTimeStamp = GETUTCDATE()
                            FROM Chapter c
                            INNER JOIN Book b ON b.Id = c.BookId
                            Where b.LibraryId = @LibraryId AND c.BookId = @BookId AND c.ChapterNumber = @ChapterNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, ReviewerAccountId = assignedAccountId, BookId = bookId, ChapterNumber = chapterNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetChapterById(libraryId, bookId, chapterNumber, cancellationToken);
            }
        }

        private async Task<ChapterModel> GetChapterByChapterId(int chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ChapterModel chapter = null;
                var sql = @"Select c.*, cc.*, a.*, ar.*
                            From Chapter c
                            Inner Join Book b On b.Id = c.BookId
                            Left Outer Join ChapterContent cc On c.Id = cc.ChapterId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = c.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = c.ReviewerAccountId
                            Where c.Id = @ChapterId ";
                var command = new CommandDefinition(sql, new { ChapterId = chapterId }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ChapterModel, ChapterContentModel, AccountModel, AccountModel, ChapterModel>(command, (c, cc, wa, ra) =>
                {
                    if (chapter == null)
                    {
                        chapter = c;
                        chapter.WriterAccountName = wa?.Name;
                        chapter.ReviewerAccountName = ra?.Name;
                    }

                    if (cc != null)
                    {
                        var content = chapter.Contents.SingleOrDefault(x => x.Id == cc.Id);
                        if (content == null)
                        {
                            cc.ChapterNumber = c.ChapterNumber;
                            cc.BookId = c.BookId;
                            chapter.Contents.Add(cc);
                        }
                    }

                    return chapter;
                });

                return chapter;
            }
        }
    }
}
