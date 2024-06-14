using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
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
