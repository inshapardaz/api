using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class ChapterDataHelper
    {
        public static void AddChapter(this IDbConnection connection, ChapterDto chapter)
        {
            throw new NotImplementedException();
        }

        public static void AddChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            throw new NotImplementedException();
        }

        public static void DeleteChapters(this IDbConnection connection, IEnumerable<ChapterDto> chapters)
        {
            throw new NotImplementedException();
        }

        public static ChapterDto GetChapterById(this IDbConnection connection, int libraryId, int id)
        {
            return connection.QuerySingleOrDefault<ChapterDto>("Select * From Library.Chapter Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<ChapterDto> GetChaptersByBook(this IDbConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public static ChapterContentDto GetContentById(this IDbConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<ChapterContentDto> GetContentByChapter(this IDbConnection connection, int chapterId)
        {
            throw new NotImplementedException();
        }
    }
}
