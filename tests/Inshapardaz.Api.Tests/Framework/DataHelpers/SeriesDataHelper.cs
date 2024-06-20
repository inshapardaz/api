using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface ISeriesTestRepository
    {
        void AddSeries(SeriesDto series);


        void AddSerieses(IEnumerable<SeriesDto> serieses);


        void DeleteSeries(IEnumerable<SeriesDto> serieses);


        SeriesDto GetSeriesById(int id);


        SeriesDto GetSeriesByBook(int id);


        int GetBookCountBySeries(int seriesId);


        string GetSeriesImageUrl(int id);


        FileDto GetSeriesImage(int id);

    }


    public class MySqlSeriesTestRepository : ISeriesTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlSeriesTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddSeries(SeriesDto series)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Series (`Name`, `Description`, ImageId, LibraryId) VALUES (@Name, @Description, @ImageId, @LibraryId); SELECT LAST_INSERT_ID();", series);
                series.Id = id;
            }
        }

        public void AddSerieses(IEnumerable<SeriesDto> serieses)
        {
            foreach (var series in serieses)
            {
                AddSeries(series);
            }
        }

        public void DeleteSeries(IEnumerable<SeriesDto> serieses)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Series WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = serieses.Select(a => a.Id) });
            }
        }

        public SeriesDto GetSeriesById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<SeriesDto>("SELECT * FROM Series WHERE Id = @Id", new { Id = id });
            }
        }

        public SeriesDto GetSeriesByBook(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<SeriesDto>(@"SELECT s.* FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE b.Id = @BookId ", new { BookId = id });
            }
        }

        public int GetBookCountBySeries(int seriesId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<int>(@"SELECT COUNT(*) FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE s.Id = @SeriesId", new { SeriesId = seriesId });
            }
        }

        public string GetSeriesImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath from `File` f
                        INNER JOIN Series s ON f.Id = s.ImageId
                        WHERE s.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetSeriesImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from `File` f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }
    }

    public class SqlServerSeriesTestRepository : ISeriesTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerSeriesTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddSeries(SeriesDto series)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Series (Name, [Description], ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @Description, @ImageId, @LibraryId)", series);
                series.Id = id;
            }
        }

        public void AddSerieses(IEnumerable<SeriesDto> serieses)
        {
            foreach (var series in serieses)
            {
                AddSeries(series);
            }
        }

        public void DeleteSeries(IEnumerable<SeriesDto> serieses)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Series WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = serieses.Select(a => a.Id) });
            }
        }

        public SeriesDto GetSeriesById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<SeriesDto>("SELECT * FROM Series WHERE Id = @Id", new { Id = id });
            }
        }

        public SeriesDto GetSeriesByBook(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<SeriesDto>(@"SELECT s.* FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE b.Id = @BookId ", new { BookId = id });
            }
        }

        public int GetBookCountBySeries(int seriesId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<int>(@"SELECT COUNT(*) FROM Series s
                                INNER JOIN Book b ON s.Id = b.SeriesId
                                WHERE s.Id = @SeriesId", new { SeriesId = seriesId });
            }
        }

        public string GetSeriesImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath from [File] f
                        INNER JOIN Series s ON f.Id = s.ImageId
                        WHERE s.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetSeriesImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from [File] f
                        Inner Join Series s ON f.Id = s.ImageId
                        Where s.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }
    }
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
