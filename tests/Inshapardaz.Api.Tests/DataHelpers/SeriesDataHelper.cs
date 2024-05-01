using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class SeriesDataHelper
    {
        public static void AddSeries(this IDbConnection connection, SeriesDto series)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Series (Name, [Description], ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @Description, @ImageId, @LibraryId)", series);
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
            var sql = "Delete From Series Where Id IN @Ids";
            connection.Execute(sql, new { Ids = serieses.Select(a => a.Id) });
        }

        public static SeriesDto GetSeriesById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>("Select * From Series Where Id = @Id", new { Id = id });
        }

        public static SeriesDto GetSeriesByBook(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>(@"Select s.* From Series s
                                Inner Join Book b ON s.Id = b.SeriesId
                                Where b.Id = @BookId ", new { BookId = id });
        }

        public static int GetBookCountBySeries(this IDbConnection connection, int seriesId)
        {
            return connection.QuerySingleOrDefault<int>(@"Select Count(*) From Series s
                                Inner Join Book b ON s.Id = b.SeriesId
                                Where s.Id = @SeriesId", new { SeriesId = seriesId });
        }

        public static string GetSeriesImageUrl(this IDbConnection connection, int id)
        {
            var sql = @"Select f.FilePath from [File] f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }

        public static FileDto GetSeriesImage(this IDbConnection connection, int id)
        {
            var sql = @"Select f.* from [File] f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }
    }
}
