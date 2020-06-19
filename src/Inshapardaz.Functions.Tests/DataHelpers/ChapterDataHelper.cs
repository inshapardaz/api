using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class ChapterDataHelper
    {
        public static void AddChapter(this IDbConnection connection, ChapterDto chapter)
        {
            var sql = "Insert Into Library.Chapter (Title, BookId, ChapterNumber) OUTPUT Inserted.Id VALUES (@Title, @BookId, @ChapterNumber)";
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
            var sql = "Insert Into Library.ChapterContent (ChapterId, FileId, Language) OUTPUT Inserted.Id VALUES (@ChapterId, @FileId, @Language)";
            var id = connection.ExecuteScalar<int>(sql, content);
            content.Id = id;
        }

        public static void DeleteChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            var sql = "Delete From Library.Chapter Where Id IN @Ids";
            connection.Execute(sql, new { Ids = chapters.Select(a => a.Id) });
        }

        public static ChapterDto GetChapterById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("Select * From Library.Chapter Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<ChapterDto> GetChaptersByBook(this IDbConnection connection, int id)
        {
            return connection.Query<ChapterDto>("Select * From Library.Chapter Where BookId = @Id", new { Id = id });
        }

        public static ChapterContentDto GetChapterContentById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ChapterContentDto>("Select * From Library.ChapterContent Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<ChapterContentDto> GetContentByChapter(this IDbConnection connection, int chapterId)
        {
            return connection.Query<ChapterContentDto>("Select * From Library.ChapterContent Where ChapterId = @Id", new { Id = chapterId });
        }

        public static IEnumerable<FileDto> GetFilesByChapter(this IDbConnection connection, int chapterId)
        {
            var sql = "Select f.* From Inshapardaz.[File] f INNER JOIN Library.ChapterContent cc ON cc.FileId = f.Id Where cc.ChapterId = @Id";
            return connection.Query<FileDto>(sql, new { Id = chapterId });
        }

        public static FileDto GetFileByChapter(this IDbConnection connection, int chapterId, string language, string mimetype)
        {
            var sql = @"Select f.* From Inshapardaz.[File] f
                        INNER JOIN Library.ChapterContent cc ON cc.FileId = f.Id
                        Where cc.ChapterId = @Id AND cc.language = @Language AND f.MimeType = @MimeType";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = chapterId, language = language, MimeType = mimetype });
        }
    }
}
