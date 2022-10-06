using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class ChapterDataHelper
    {
        public static void AddChapter(this IDbConnection connection, ChapterDto chapter)
        {
            var sql = @"Insert Into Chapter (Title, BookId, ChapterNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                      OUTPUT Inserted.Id 
                      VALUES (@Title, @BookId, @ChapterNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)";
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
            var sql = "Insert Into ChapterContent (ChapterId, Language, Text) OUTPUT Inserted.Id VALUES (@ChapterId, @Language, @Text)";
            var id = connection.ExecuteScalar<int>(sql, content);
            content.Id = id;
        }

        public static void DeleteChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            var sql = "Delete From Chapter Where Id IN @Ids";
            connection.Execute(sql, new { Ids = chapters.Select(a => a.Id) });
        }

        public static void DeleteChapterContents(this IDbConnection connection, IEnumerable<ChapterContentDto> chapters)
        {
            var sql = "Delete From ChapterContent Where Id IN @Ids";
            connection.Execute(sql, new { Ids = chapters.Select(c => c.Id) });
        }

        public static ChapterDto GetChapterById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("Select * From Chapter Where Id = @Id", new { Id = id });
        }

        public static ChapterDto GetChapterByBookAndChapter(this IDbConnection connection, int bookId, int chapterId)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("Select * From Chapter Where BookId = @bookId AND Id = @chapterId", new { bookId, chapterId });
        }

        public static IEnumerable<ChapterDto> GetChaptersByBook(this IDbConnection connection, int id)
        {
            return connection.Query<ChapterDto>("Select * From Chapter Where BookId = @Id", new { Id = id });
        }

        public static ChapterContentDto GetChapterContentById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ChapterContentDto>("Select * From ChapterContent Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<ChapterContentDto> GetContentByChapter(this IDbConnection connection, int chapterId)
        {
            return connection.Query<ChapterContentDto>("Select * From ChapterContent Where ChapterId = @Id", new { Id = chapterId });
        }

        public static IEnumerable<FileDto> GetFilesByChapter(this IDbConnection connection, int chapterId)
        {
            var sql = "Select f.* From [File] f INNER JOIN ChapterContent cc ON cc.FileId = f.Id Where cc.ChapterId = @Id";
            return connection.Query<FileDto>(sql, new { Id = chapterId });
        }

        public static FileDto GetFileByChapter(this IDbConnection connection, int chapterId, string language, string mimetype)
        {
            var sql = @"Select f.* From [File] f
                        INNER JOIN ChapterContent cc ON cc.FileId = f.Id
                        Where cc.ChapterId = @Id AND cc.language = @Language AND f.MimeType = @MimeType";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = chapterId, language = language, MimeType = mimetype });
        }
    }
}
