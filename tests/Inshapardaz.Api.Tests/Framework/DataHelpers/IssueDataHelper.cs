using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IIssueTestRepository
    {
        void AddIssue(IssueDto issue);

        void AddIssues(IEnumerable<IssueDto> issues);

        void AddIssueFiles(int issueId, IEnumerable<IssueContentDto> contentDto);
        void AddIssueFile(int issueId, IssueContentDto contentDto);

        IssueDto GetIssueById(int issueId);

        IEnumerable<IssueDto> GetIssuesByPeriodical(int periodicalId);

        string GetIssueImageUrl(int issueId);

        FileDto GetIssueImage(int issueId);

        void DeleteIssues(IEnumerable<IssueDto> issues);

        IEnumerable<IssueContentDto> GetIssueContents(int issueId);

        IssueContentDto GeIssueContent(int issueId, string language, string mimetype);

        string GetIssueContentPath(long issueId, string language, string mimetype);
        IssueContentDto GetIssueContent(long issueId);
    }

    public class MySqlIssueTestRepository : IIssueTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlIssueTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssue(IssueDto issue)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate, IsPublic, Status)
                        VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate, @IsPublic, @Status);
                        SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, issue);
                issue.Id = id;
            }
        }
        public void AddIssues(IEnumerable<IssueDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssue(issue);
            }
        }

        public void AddIssueFiles(int issueId, IEnumerable<IssueContentDto> contentDto) =>
            contentDto.ForEach(f => AddIssueFile(issueId, f));

        public void AddIssueFile(int issueId, IssueContentDto contentDto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                    VALUES (@IssueId, @FileId, @Language, @MimeType);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, new { IssueId = issueId, FileId = contentDto.FileId, Language = contentDto.Language, MimeType = contentDto.MimeType });
                contentDto.Id = id;
            }
        }

        public IssueDto GetIssueById(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Issue WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueDto>(sql, new { Id = issueId });
            }
        }

        public IEnumerable<IssueDto> GetIssuesByPeriodical(int periodicalId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.* FROM Issue i
                        INNER JOIN Periodical p on p.Id = i.PeriodicalId
                        WHERE i.PeriodicalId = @PeriodicalId";
                return connection.Query<IssueDto>(sql, new { PeriodicalId = periodicalId });
            }
        }

        public string GetIssueImageUrl(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM `File` f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = issueId });
            }
        }

        public FileDto GetIssueImage(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* from `File` f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = issueId });
            }
        }

        public void DeleteIssues(IEnumerable<IssueDto> issues)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Issue WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = issues.Select(f => f.Id) });
            }
        }

        public IEnumerable<IssueContentDto> GetIssueContents(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT ic.*, f.MimeType From IssueContent ic
                    INNER Join Issue i ON i.Id = ic.IssueId
                    INNER Join `File` f ON f.Id = ic.FileId
                    Where i.Id = @IssueId";

                return connection.Query<IssueContentDto>(sql, new
                {
                    IssueId = issueId
                });
            }
        }

        public IssueContentDto GeIssueContent(int issueId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where i.Id = @IssueId AND ic.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
                {
                    IssueId = issueId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public string GetIssueContentPath(long issueId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT f.FilePath 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join `File` f ON f.Id = ic.FileId
                           WHERE ic.Id = @Id 
                                AND ic.Language = @Language 
                                AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<string>(sql, new
                {
                    Id = issueId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public IssueContentDto GetIssueContent(long issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT * 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join `File` f ON f.Id = ic.FileId
                           WHERE ic.Id = @IssueId";

                return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
                {
                    IssueId = issueId
                });
            }
        }
    }

    public class SqlServerIssueTestRepository : IIssueTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerIssueTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssue(IssueDto issue)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate, IsPublic, Status)
                        OUTPUT INSERTED.ID
                        VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate, @IsPublic, @Status)";
                var id = connection.ExecuteScalar<int>(sql, issue);
                issue.Id = id;
            }
        }
        public void AddIssues(IEnumerable<IssueDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssue(issue);
            }
        }

        public void AddIssueFiles(int issueId, IEnumerable<IssueContentDto> contentDto) =>
            contentDto.ForEach(f => AddIssueFile(issueId, f));

        public void AddIssueFile(int issueId, IssueContentDto contentDto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                    OUTPUT INSERTED.Id
                    VALUES (@IssueId, @FileId, @Language, @MimeType)";
                var id = connection.ExecuteScalar<int>(sql, new { IssueId = issueId, FileId = contentDto.FileId, Language = contentDto.Language, MimeType = contentDto.MimeType });
                contentDto.Id = id;
            }
        }

        public IssueDto GetIssueById(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Issue WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueDto>(sql, new { Id = issueId });
            }
        }

        public IEnumerable<IssueDto> GetIssuesByPeriodical(int periodicalId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.* FROM Issue i
                        INNER JOIN Periodical p on p.Id = i.PeriodicalId
                        WHERE i.PeriodicalId = @PeriodicalId";
                return connection.Query<IssueDto>(sql, new { PeriodicalId = periodicalId });
            }
        }

        public string GetIssueImageUrl(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM [File] f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = issueId });
            }
        }

        public FileDto GetIssueImage(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* from [File] f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = issueId });
            }
        }

        public void DeleteIssues(IEnumerable<IssueDto> issues)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Issue WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = issues.Select(f => f.Id) });
            }
        }

        public IEnumerable<IssueContentDto> GetIssueContents(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT ic.*, f.MimeType From IssueContent ic
                    INNER Join Issue i ON i.Id = ic.IssueId
                    INNER Join [File] f ON f.Id = ic.FileId
                    Where i.Id = @IssueId";

                return connection.Query<IssueContentDto>(sql, new
                {
                    IssueId = issueId
                });
            }
        }

        public IssueContentDto GeIssueContent(int issueId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where i.Id = @IssueId AND ic.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
                {
                    IssueId = issueId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public string GetIssueContentPath(long issueId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT f.FilePath 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join [File] f ON f.Id = ic.FileId
                           WHERE ic.Id = @Id 
                                AND ic.Language = @Language 
                                AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<string>(sql, new
                {
                    Id = issueId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public IssueContentDto GetIssueContent(long issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT * 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join [File] f ON f.Id = ic.FileId
                           WHERE ic.Id = @IssueId";

                return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
                {
                    IssueId = issueId
                });
            }
        }
    }


    public static class IssueDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddIssue(this IDbConnection connection, IssueDto issue)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"INSERT INTO Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate, IsPublic)
                        OUTPUT INSERTED.ID
                        VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate, @IsPublic)"
                : @"INSERT INTO Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate, IsPublic)
                        VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate, @IsPublic);
                        SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, issue);
            issue.Id = id;
        }

        public static void AddIssues(this IDbConnection connection, IEnumerable<IssueDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssue(connection, issue);
            }
        }

        public static void AddIssueFiles(this IDbConnection connection, int issueId, IEnumerable<IssueContentDto> contentDto) =>
            contentDto.ForEach(f => connection.AddIssueFile(issueId, f));

        public static void AddIssueFile(this IDbConnection connection, int issueId, IssueContentDto contentDto)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                    OUTPUT INSERTED.Id
                    VALUES (@IssueId, @FileId, @Language, @MimeType)"
                : @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                    VALUES (@IssueId, @FileId, @Language, @MimeType);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, new { IssueId = issueId, FileId = contentDto.FileId, Language = contentDto.Language, MimeType = contentDto.MimeType });
            contentDto.Id = id;
        }

        public static IssueDto GetIssueById(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT * FROM Issue WHERE Id = @Id";
            return connection.QuerySingleOrDefault<IssueDto>(sql, new { Id = issueId });
        }

        public static IEnumerable<IssueDto> GetIssuesByPeriodical(this IDbConnection connection, int periodicalId)
        {
            var sql = @"SELECT i.* FROM Issue i
                        INNER JOIN Periodical p on p.Id = i.PeriodicalId
                        WHERE i.PeriodicalId = @PeriodicalId";
            return connection.Query<IssueDto>(sql, new { PeriodicalId = periodicalId });
        }
        public static string GetIssueImageUrl(this IDbConnection connection, int issueId)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.FilePath FROM [File] f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id"
                : @"SELECT f.FilePath FROM `File` f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = issueId });
        }

        public static FileDto GetIssueImage(this IDbConnection connection, int issueId)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT f.* from [File] f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id"
                : @"SELECT f.* from `File` f
                    INNER JOIN Issue i ON f.Id = i.ImageId
                    WHERE i.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = issueId });
        }

        public static void DeleteIssues(this IDbConnection connection, IEnumerable<IssueDto> issues)
        {
            var sql = "DELETE FROM Issue WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = issues.Select(f => f.Id) });
        }

        public static IEnumerable<IssueContentDto> GetIssueContents(this IDbConnection connection, int issueId)
        {
            string sql = _dbType == DatabaseTypes.SqlServer
                ? @"SELECT ic.*, f.MimeType From IssueContent ic
                    INNER Join Issue i ON i.Id = ic.IssueId
                    INNER Join [File] f ON f.Id = ic.FileId
                    Where i.Id = @IssueId"
                : @"SELECT ic.*, f.MimeType From IssueContent ic
                    INNER Join Issue i ON i.Id = ic.IssueId
                    INNER Join `File` f ON f.Id = ic.FileId
                    Where i.Id = @IssueId";

            return connection.Query<IssueContentDto>(sql, new
            {
                IssueId = issueId
            });
        }

        public static IssueContentDto GeIssueContent(this IDbConnection connection, int issueId, string language, string mimetype)
        {
            string sql = @"Select * From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where i.Id = @IssueId AND ic.Language = @Language AND f.MimeType = @MimeType";

            return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
            {
                IssueId = issueId,
                Language = language,
                MimeType = mimetype
            });
        }

        public static string GetIssueContentPath(this IDbConnection connection, long issueId, string language, string mimetype)
        {
            string sql = @"SELECT f.FilePath 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join `File` f ON f.Id = ic.FileId
                           WHERE ic.Id = @Id 
                                AND ic.Language = @Language 
                                AND f.MimeType = @MimeType";

            return connection.QuerySingleOrDefault<string>(sql, new
            {
                Id = issueId,
                Language = language,
                MimeType = mimetype
            });
        }

        public static IssueContentDto GetIssueContent(this IDbConnection connection, long issueId)
        {
            string sql = @"SELECT * 
                           FROM IssueContent ic
                               INNER Join Issue i ON i.Id = ic.IssueId
                               INNER Join `File` f ON f.Id = ic.FileId
                           WHERE ic.Id = @IssueId";

            return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
            {
                IssueId = issueId
            });
        }
    }
}
