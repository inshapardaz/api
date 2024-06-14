using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class ChapterDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddChapter(this IDbConnection connection, ChapterDto chapter)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"INSERT INTO Chapter (Title, BookId, ChapterNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                      OUTPUT Inserted.Id 
                      VALUES (@Title, @BookId, @ChapterNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)"
                : @"INSERT INTO Chapter (Title, BookId, ChapterNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                      VALUES (@Title, @BookId, @ChapterNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, chapter);
            chapter.Id = id;
        }

        public static void AddChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            foreach (var chapter in chapters)
            {
                connection.AddChapter(chapter);
            }
        }

        public static void AddChapterContent(this IDbConnection connection, ChapterContentDto content)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? "INSERT INTO ChapterContent (ChapterId, Language, FileId) OUTPUT Inserted.Id VALUES (@ChapterId, @Language, @FileId)"
                : @"INSERT INTO ChapterContent (ChapterId, `Language`, FileId) VALUES (@ChapterId, @Language, @FileId);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, content);
            content.Id = id;
        }

        public static void DeleteChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            var sql = "DELETE FROM Chapter WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = chapters.Select(a => a.Id) });
        }

        public static void DeleteChapterContents(this IDbConnection connection, IEnumerable<ChapterContentDto> chapters)
        {
            var sql = "DELETE FROM ChapterContent WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = chapters.Select(c => c.Id) });
        }

        public static ChapterDto GetChapterById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("SELECT * FROM Chapter WHERE Id = @Id", new { Id = id });
        }

        public static ChapterDto GetChapterByBookAndChapter(this IDbConnection connection, int bookId, long chapterId)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("SELECT * FROM Chapter WHERE BookId = @bookId AND Id = @chapterId", new { bookId, chapterId });
        }

        public static IEnumerable<ChapterDto> GetChaptersByBook(this IDbConnection connection, int id)
        {
            return connection.Query<ChapterDto>("SELECT * FROM Chapter WHERE BookId = @Id ORDER BY ChapterNumber", new { Id = id });
        }

        public static ChapterContentDto GetChapterContentById(this IDbConnection connection, long id)
        {
            return connection.QuerySingleOrDefault<ChapterContentDto>("SELECT * FROM ChapterContent WHERE Id = @Id", new { Id = id });
        }

        public static IEnumerable<ChapterContentDto> GetContentByChapter(this IDbConnection connection, long chapterId)
        {
            return connection.Query<ChapterContentDto>("SELECT * FROM ChapterContent WHERE ChapterId = @Id", new { Id = chapterId });
        }

        public static IEnumerable<FileDto> GetFilesByChapter(this IDbConnection connection, long chapterId)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? "SELECT f.* FROM [File] f INNER JOIN ChapterContent cc ON cc.FileId = f.Id WHERE cc.ChapterId = @Id"
                : "SELECT f.* FROM `File` f INNER JOIN ChapterContent cc ON cc.FileId = f.Id WHERE cc.ChapterId = @Id";
            return connection.Query<FileDto>(sql, new { Id = chapterId });
        }

        public static FileDto GetFileByChapter(this IDbConnection connection, int chapterId, string language, string mimetype)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.* From [File] f
                        INNER JOIN ChapterContent cc ON cc.FileId = f.Id
                        WHERE cc.ChapterId = @Id AND cc.language = @Language AND f.MimeType = @MimeType"
                : @"SELECT f.* From `File` f
                        INNER JOIN ChapterContent cc ON cc.FileId = f.Id
                        WHERE cc.ChapterId = @Id AND cc.language = @Language AND f.MimeType = @MimeType";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = chapterId, language = language, MimeType = mimetype });
        }
    }
}
