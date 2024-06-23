using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Namotion.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IPeriodicalTestRepository
    {
        void AddPeriodical(PeriodicalDto periodical);
        void AddPeriodicals(IEnumerable<PeriodicalDto> Periodicals);
        void DeletePeriodicals(IEnumerable<PeriodicalDto> periodicals);
        void DeletePeriodical(int periodicalId);
        PeriodicalDto GetPeriodicalById(int id);
        string GetPeriodicalImageUrl(int id);
        FileDto GetPeriodicalImage(int id);
        void UpdatePeriodical(PeriodicalDto periodical);
    }

    public class MySqlPeriodicalTestRepository : IPeriodicalTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlPeriodicalTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddPeriodical(PeriodicalDto periodical)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Periodical (Title, `Description`, `Language`, ImageId, LibraryId, Frequency) 
                    VALUES (@Title, @Description, @Language, @ImageId, @LibraryId, @Frequency);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, periodical);
                periodical.Id = id;
            }
        } 
        
        public void UpdatePeriodical(PeriodicalDto periodical)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE Periodical SET 
                                Title = @Title, 
                                `Description` = @Description, 
                                `Language` = @Language, 
                                ImageId = @ImageId, 
                                LibraryId = @LibraryId, 
                                Frequency = @Frequency
                    WHERE Id = @Id";
                connection.Execute(sql, periodical);
            }
        }

        public void AddPeriodicals(IEnumerable<PeriodicalDto> Periodicals)
        {
            foreach (var periodical in Periodicals)
            {
                AddPeriodical(periodical);
            }
        }

        public void DeletePeriodicals(IEnumerable<PeriodicalDto> periodicals)
        {
            if (periodicals is null) return;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Periodical WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = periodicals.Select(a => a.Id) });
            }
        }

        public void DeletePeriodical(int periodicalId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Periodical WHERE Id = @Id";
                connection.Execute(sql, new { Id = periodicalId });
            }
        }

        public PeriodicalDto GetPeriodicalById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<PeriodicalDto>("SELECT * FROM Periodical WHERE Id = @Id", new { Id = id });
            }
        }

        public string GetPeriodicalImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM `File` f
                        INNER JOIN Periodical p ON f.Id = p.ImageId
                        WHERE p.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetPeriodicalImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* FROM `File` f
                    INNER JOIN Periodical p ON f.Id = p.ImageId
                    WHERE p.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }
    }

    public class SqlServerPeriodicalTestRepository : IPeriodicalTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerPeriodicalTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddPeriodical(PeriodicalDto periodical)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Periodical (Title, [Description], Language, ImageId, LibraryId, Frequency) 
                    OUTPUT Inserted.Id 
                    VALUES (@Title, @Description, @Language, @ImageId, @LibraryId, @Frequency)";
                var id = connection.ExecuteScalar<int>(sql, periodical);
                periodical.Id = id;
            }
        }

        public void UpdatePeriodical(PeriodicalDto periodical)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE Periodical SET 
                                Title = @Title, 
                                [Description] = @Description, 
                                [Language] = @Language, 
                                ImageId = @ImageId, 
                                LibraryId = @LibraryId, 
                                Frequency = @Frequency
                    WHERE Id = @Id";
                connection.Execute(sql, periodical);
            }
        }

        public void AddPeriodicals(IEnumerable<PeriodicalDto> Periodicals)
        {
            foreach (var periodical in Periodicals)
            {
                AddPeriodical(periodical);
            }
        }

        public void DeletePeriodicals(IEnumerable<PeriodicalDto> periodicals)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Periodical WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = periodicals.Select(a => a.Id) });
            }
        }

        public void DeletePeriodical(int periodicalId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Periodical WHERE Id = @Id";
                connection.Execute(sql, new { Id = periodicalId });
            }
        }

        public PeriodicalDto GetPeriodicalById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<PeriodicalDto>("SELECT * FROM Periodical WHERE Id = @Id", new { Id = id });
            }
        }

        public string GetPeriodicalImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM [File] f
                        INNER JOIN Periodical p ON f.Id = p.ImageId
                        WHERE p.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetPeriodicalImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* FROM [File] f
                    INNER JOIN Periodical p ON f.Id = p.ImageId
                    WHERE p.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }
    }

    public static class PeriodicalDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddPeriodical(this IDbConnection connection, PeriodicalDto periodical)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"INSERT INTO Periodical (Title, [Description], Language, ImageId, LibraryId, Frequency) 
                    OUTPUT Inserted.Id 
                    VALUES (@Title, @Description, @Language, @ImageId, @LibraryId, @Frequency)"
                : @"INSERT INTO Periodical (Title, `Description`, `Language`, ImageId, LibraryId, Frequency) 
                    VALUES (@Title, @Description, @Language, @ImageId, @LibraryId, @Frequency);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, periodical);
            periodical.Id = id;
        }

        public static void AddPeriodicals(this IDbConnection connection, IEnumerable<PeriodicalDto> Periodicals)
        {
            foreach (var periodical in Periodicals)
            {
                AddPeriodical(connection, periodical);
            }
        }

        public static void DeletePeriodicals(this IDbConnection connection, IEnumerable<PeriodicalDto> periodicals)
        {
            var sql = "DELETE FROM Periodical WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = periodicals.Select(a => a.Id) });
        }

        public static void DeletePeriodical(this IDbConnection connection, int periodicalId)
        {
            var sql = "DELETE FROM Periodical WHERE Id = @Id";
            connection.Execute(sql, new { Id = periodicalId });
        }

        public static PeriodicalDto GetPeriodicalById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<PeriodicalDto>("SELECT * FROM Periodical WHERE Id = @Id", new { Id = id });
        }

        public static string GetPeriodicalImageUrl(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.FilePath FROM [File] f
                        INNER JOIN Periodical p ON f.Id = p.ImageId
                        WHERE p.Id = @Id"
                : @"SELECT f.FilePath FROM `File` f
                        INNER JOIN Periodical p ON f.Id = p.ImageId
                        WHERE p.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }

        public static FileDto GetPeriodicalImage(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.* FROM [File] f
                    INNER JOIN Periodical p ON f.Id = p.ImageId
                    WHERE p.Id = @Id"
                : @"SELECT f.* FROM `File` f
                    INNER JOIN Periodical p ON f.Id = p.ImageId
                    WHERE p.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }
    }
}
