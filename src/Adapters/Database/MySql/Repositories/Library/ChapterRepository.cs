using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class ChapterRepository : IChapterRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public ChapterRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<ChapterModel> AddChapter(int libraryId, int bookId, ChapterModel chapter, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO Chapter (Title, BookId, ChapterNumber, `Status`, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp ) 
                            VALUES (@Title, @BookId, @ChapterNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp);
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new
            {
                Title = chapter.Title,
                BookId = bookId,
                ChapterNumber = chapter.ChapterNumber,
                Status = chapter.Status,
                WriterAccountId = chapter.WriterAccountId,
                WriterAssignTimeStamp = chapter.WriterAssignTimeStamp,
                ReviewerAccountId = chapter.ReviewerAccountId,
                ReviewerAssignTimeStamp = chapter.ReviewerAssignTimeStamp
            }, cancellationToken: cancellationToken) ;
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        await ReorderChapters(libraryId, bookId, cancellationToken);
        return await GetChapterByChapterId(id, cancellationToken);
    }

    public async Task UpdateChapter(int libraryId, int bookId, int oldChapterNumber, ChapterModel chapter, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Chapter C
                                INNER JOIN Book b ON b.Id = C.BookId
                            SET C.Title = @Title, C.BookId = @BookId, C.ChapterNumber = @ChapterNumber, C.Status = @Status
                            WHERE C.chapterNumber = @OldChapterNumber 
                                AND C.BookId = @OldBookId 
                                AND b.LibraryId = @LibraryId";
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
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Chapter C
                            INNER JOIN Book b ON b.Id = C.BookId
                            SET C.ChapterNumber = @ChapterNumber
                            WHERE C.Id = @Id 
                                AND C.BookId = @BookId 
                                AND b.LibraryId = @LibraryId";
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
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql1 = @"UPDATE BookPage AS bp
                                INNER JOIN Chapter c ON c.Id = bp.ChapterId
                             SET ChapterId = NULL 
                             WHERE c.BookId = @BookId 
                                AND c.ChapterNumber = @ChapterNumber";

            var command1 = new CommandDefinition(sql1, new
            {
                BookId = bookId,
                ChapterNumber = chapterNumber
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command1);

            var sql = @"DELETE c FROM Chapter c
                            INNER JOIN Book b ON b.Id = c.BookId
                            WHERE c.chapterNumber = @ChapterNumber 
                                AND c.BookId = @BookId 
                                AND b.LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                ChapterNumber = chapterNumber
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }

        await ReorderChapters(libraryId, bookId, cancellationToken);
    }

    public async Task<IEnumerable<ChapterModel>> GetChaptersByBook(int libraryId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var chapters = new Dictionary<long, ChapterModel>();

            var sql = @"SELECT c.*, cc.Id as ContentId, cc.Language As ContentLanguage,
                            a.Name As WriterAccountName, ar.Name As ReviewerAccountName
                            FROM Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                                LEFT OUTER JOIN ChapterContent cc ON c.Id = cc.ChapterId
                                LEFT OUTER JOIN Accounts a ON a.Id = c.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = c.ReviewerAccountId
                            WHERE b.Id = @BookId 
                                AND b.LibraryId = @LibraryId
                            ORDER BY c.ChapterNumber";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.QueryAsync<ChapterModel, long?, string, string, string, ChapterModel>(command, (c, contentId, contentLangugage, WriterAccountName, ReviewerAccountName) =>
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
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            ChapterModel chapter = null;
            var sql = @"SELECT c.*, cc.*, a.*, ar.*
                            FROM Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                                LEFT OUTER JOIN ChapterContent cc ON c.Id = cc.ChapterId
                                LEFT OUTER JOIN Accounts a ON a.Id = c.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = c.ReviewerAccountId
                            WHERE c.chapterNumber = @ChapterNumber 
                                AND b.Id = @BookId 
                                AND b.LibraryId = @LibraryId";
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
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT cc.*, c.chapterNumber, b.Id AS BookId
                            FROM Chapter c
                            INNER JOIN Book b ON b.Id = c.BookId
                            LEFT OUTER JOIN ChapterContent cc ON c.Id = cc.ChapterId
                            WHERE c.chapterNumber = @ChapterId 
                                AND b.Id = @BookId 
                                AND b.LibraryId = @LibraryId 
                                AND cc.Language = @Language";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, ChapterId = chapterNumber, Language = language }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ChapterContentModel>(command);
        }
    }

    public async Task<IEnumerable<ChapterContentModel>> GetChapterContents(int libraryId, int bookId, int chapterNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT cc.*, c.chapterNumber, b.Id AS BookId 
                            FROM Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                                LEFT OUTER JOIN ChapterContent cc ON c.Id = cc.ChapterId
                            WHERE c.chapterNumber = @ChapterId
                                AND b.Id = @BookId 
                                AND b.LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, ChapterId = chapterNumber }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<ChapterContentModel>(command);
        }
    }

    public async Task<ChapterContentModel> AddChapterContent(int libraryId, ChapterContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO ChapterContent (ChapterId, `Language`, Text, FileId)
                            VALUES (@ChapterId, @Language, @Text, @FileId)";
            var command = new CommandDefinition(sql, new { 
                ChapterId = content.ChapterId, 
                Language = content.Language, 
                Text = content.Text,
                FileId = content.FileId,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetChapterContent(libraryId, content.BookId, content.ChapterNumber, content.Language, cancellationToken);
        }
    }

    public async Task UpdateChapterContent(int libraryId, int bookId, int chapterNumber, string language, string text, long? fileId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE ChapterContent cc
                                INNER JOIN Chapter c ON c.Id = cc.ChapterId
                                INNER JOIN Book b ON b.Id = c.BookId
                            SET Text = @Text, FileId = @FileId
                            WHERE c.ChapterNumber = @ChapterNumber 
                                AND b.LibraryId = @LibraryId 
                                AND b.Id = @BookId 
                                AND cc.Language = @Language";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                ChapterNumber = chapterNumber,
                Language = language,
                Text = text,
                FileId = fileId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteChapterContentById(int libraryId, int bookId, int chapterNumber, string language, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE cc
                            FROM ChapterContent cc
                                INNER JOIN Chapter c ON c.Id = cc.ChapterId
                                INNER JOIN Book b ON b.Id = C.BookId
                            WHERE c.chapterNumber = @ChapterNumber 
                                AND b.LibraryId = @LibraryId
                                AND b.Id = @BookId 
                                AND cc.Language = @Language";
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
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT c.Id, row_number() OVER (ORDER BY c.ChapterNumber) AS 'ChapterNumber'
                            FROM Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                            WHERE c.BookId = @BookId 
                                AND b.LibraryId = @LibraryId
                            ORDER BY c.ChapterNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId
            }, cancellationToken: cancellationToken);
            var newOrder = await connection.QueryAsync(command);

            var sql2 = @"UPDATE Chapter
                            SET ChapterNumber = @ChapterNumber
                            WHERE Id = @Id";
            var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command2);
        }
    }

    public async Task<ChapterModel> UpdateWriterAssignment(int libraryId, int bookId, int chapterNumber, int? assignedAccountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                            SET c.WriterAccountId = @WriterAccountId, c.WriterAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE b.LibraryId = @LibraryId 
                                AND c.BookId = @BookId 
                                AND c.ChapterNumber = @ChapterNumber";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, WriterAccountId = assignedAccountId, BookId = bookId, ChapterNumber = chapterNumber }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetChapterById(libraryId, bookId, chapterNumber, cancellationToken);
        }
    }

    public async Task<ChapterModel> UpdateReviewerAssignment(int libraryId, int bookId, int chapterNumber, int? assignedAccountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                            SET c.ReviewerAccountId = @ReviewerAccountId, c.ReviewerAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE b.LibraryId = @LibraryId 
                                AND c.BookId = @BookId 
                                AND c.ChapterNumber = @ChapterNumber";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, ReviewerAccountId = assignedAccountId, BookId = bookId, ChapterNumber = chapterNumber }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetChapterById(libraryId, bookId, chapterNumber, cancellationToken);
        }
    }

    private async Task<ChapterModel> GetChapterByChapterId(int chapterId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            ChapterModel chapter = null;
            var sql = @"SELECT c.*, cc.*, a.*, ar.*
                            FROM Chapter c
                                INNER JOIN Book b ON b.Id = c.BookId
                                LEFT OUTER JOIN ChapterContent cc ON c.Id = cc.ChapterId
                                LEFT OUTER JOIN Accounts a ON a.Id = c.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = c.ReviewerAccountId
                            WHERE c.Id = @ChapterId ";
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
