using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class SeriesDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddSeries(this IDbConnection connection, SeriesDto series)
        {
            var id = _dbType == DatabaseTypes.SqlServer 
                ? connection.ExecuteScalar<int>("INSERT INTO Series (Name, [Description], ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @Description, @ImageId, @LibraryId)", series)
                : connection.ExecuteScalar<int>("INSERT INTO Series (`Name`, `Description`, ImageId, LibraryId) VALUES (@Name, @Description, @ImageId, @LibraryId); SELECT LAST_INSERT_ID();", series);
            series.Id = id;
        }

        public static void AddSerieses(this IDbConnection connection, IEnumerable<SeriesDto> serieses)
        {
            foreach (var series in serieses)
            {
                AddSeries(connection, series);
            }
        }

        public static void DeleteSeries(this IDbConnection connection, IEnumerable<SeriesDto> serieses)
        {
            var sql = "DELETE FROM Series WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = serieses.Select(a => a.Id) });
        }

        public static SeriesDto GetSeriesById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>("SELECT * FROM Series WHERE Id = @Id", new { Id = id });
        }

        public static SeriesDto GetSeriesByBook(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>(@"SELECT s.* FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE b.Id = @BookId ", new { BookId = id });
        }

        public static int GetBookCountBySeries(this IDbConnection connection, int seriesId)
        {
            return connection.QuerySingleOrDefault<int>(@"SELECT COUNT(*) FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE s.Id = @SeriesId", new { SeriesId = seriesId });
        }

        public static string GetSeriesImageUrl(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.FilePath from [File] f
                        INNER JOIN Series s ON f.Id = s.ImageId
                        WHERE s.Id = @Id"
                : @"SELECT f.FilePath from `File` f
                        INNER JOIN Series s ON f.Id = s.ImageId
                        WHERE s.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }

        public static FileDto GetSeriesImage(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"Select f.* from [File] f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id"
                : @"Select f.* from `File` f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }
    }
}
