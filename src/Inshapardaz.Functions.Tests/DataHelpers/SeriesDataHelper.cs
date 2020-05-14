using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class SeriesDataHelper
    {
        public static void AddSeries(this IDbConnection connection, SeriesDto series)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Library.Series (Name, [Description], ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @Description, @ImageId, @LibraryId)", series);
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
            var sql = "Delete From Library.Series Where Id IN @Ids";
            connection.Execute(sql, new { Ids = serieses.Select(a => a.Id) });
        }

        public static SeriesDto GetSeriesById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>("Select * From Library.Series Where Id = @Id", new { Id = id });
        }

        public static SeriesDto GetSeriesByBook(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<SeriesDto>(@"Select s.* From Library.Series s
                                Inner Join Library.Book b ON s.Id = b.SeriesId
                                Where b.Id = @BookId ", new { BookId = id });
        }

        public static int GetBookCountBySeries(this IDbConnection connection, int seriesId)
        {
            return connection.QuerySingleOrDefault<int>(@"Select Count(*) From Library.Series s
                                Inner Join Library.Book b ON s.Id = b.SeriesId
                                Where s.Id = @SeriesId", new { SeriesId = seriesId });
        }
    }
}
