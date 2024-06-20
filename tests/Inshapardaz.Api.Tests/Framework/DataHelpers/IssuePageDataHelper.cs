using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{

    public interface IIssuePageTestRepository
    {
        void AddIssuePage(IssuePageDto issuePage);

        void AddIssuePages(IEnumerable<IssuePageDto> issuePages);

        void DeleteIssuePages(IEnumerable<IssuePageDto> issuePages);

        IssuePageDto GetIssuePageByNumber(int issueId, int sequenceNumber);

        IssuePageDto GetIssuePageByNumber(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber);
        IssuePageDto GetIssuePageByIssueId(int issueId, long pageId);

        IEnumerable<IssuePageDto> GetIssuePagesByIssue(int issueId);
    }

    public class MySqlIssuePageTestRepository : IIssuePageTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlIssuePageTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssuePage(IssuePageDto issuePage)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssuePage (IssueId, Text, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                    VALUES (@IssueId, @Text, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, issuePage);
                issuePage.Id = id;
            }
        }

        public void AddIssuePages(IEnumerable<IssuePageDto> issuePages)
        {
            foreach (var bookPage in issuePages)
            {
                AddIssuePage(bookPage);
            }
        }

        public void DeleteIssuePages(IEnumerable<IssuePageDto> issuePages)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Delete From IssuePage Where Id IN @Ids";
                connection.Execute(sql, new { Ids = issuePages.Select(f => f.Id) });
            }
        }

        public IssuePageDto GetIssuePageByNumber(int issueId, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { IssueId = issueId, SequenceNumber = sequenceNumber });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IssuePageDto GetIssuePageByNumber(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT ip.*
                        FROM IssuePage ip
                        INNER JOIN Issue i ON i.Id = ip.IssueId
                        Where i.PeriodicalId = @PeriodicalId 
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IssuePageDto GetIssuePageByIssueId(int issueId, long pageId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND Id = @Id";
                var command = new CommandDefinition(sql, new { IssueId = issueId, id = pageId });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IEnumerable<IssuePageDto> GetIssuePagesByIssue(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId";
                var command = new CommandDefinition(sql, new { IssueId = issueId });

                return connection.Query<IssuePageDto>(command);
            }
        }
    }

    public class SqlServerIssuePageTestRepository : IIssuePageTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerIssuePageTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssuePage(IssuePageDto issuePage)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssuePage (IssueId, Text, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                    OUTPUT INSERTED.ID
                    VALUES (@IssueId, @Text, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status)";
                var id = connection.ExecuteScalar<int>(sql, issuePage);
                issuePage.Id = id;
            }
        }

        public void AddIssuePages(IEnumerable<IssuePageDto> issuePages)
        {
            foreach (var bookPage in issuePages)
            {
                AddIssuePage(bookPage);
            }
        }

        public void DeleteIssuePages(IEnumerable<IssuePageDto> issuePages)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Delete From IssuePage Where Id IN @Ids";
                connection.Execute(sql, new { Ids = issuePages.Select(f => f.Id) });
            }
        }

        public IssuePageDto GetIssuePageByNumber(int issueId, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { IssueId = issueId, SequenceNumber = sequenceNumber });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IssuePageDto GetIssuePageByNumber(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT ip.*
                        FROM IssuePage ip
                        INNER JOIN Issue i ON i.Id = ip.IssueId
                        Where i.PeriodicalId = @PeriodicalId 
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IssuePageDto GetIssuePageByIssueId(int issueId, long pageId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND Id = @Id";
                var command = new CommandDefinition(sql, new { IssueId = issueId, id = pageId });

                return connection.QuerySingleOrDefault<IssuePageDto>(command);
            }
        }

        public IEnumerable<IssuePageDto> GetIssuePagesByIssue(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId";
                var command = new CommandDefinition(sql, new { IssueId = issueId });

                return connection.Query<IssuePageDto>(command);
            }
        }
    }

    public static class IssuePageDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddIssuePage(this IDbConnection connection, IssuePageDto issuePage)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                ? @"INSERT INTO IssuePage (IssueId, Text, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                    OUTPUT INSERTED.ID
                    VALUES (@IssueId, @Text, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status)"
                : @"INSERT INTO IssuePage (IssueId, Text, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                    VALUES (@IssueId, @Text, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, issuePage);
            issuePage.Id = id;
        }

        public static void AddIssuePages(this IDbConnection connection, IEnumerable<IssuePageDto> issuePages)
        {
            foreach (var bookPage in issuePages)
            {
                AddIssuePage(connection, bookPage);
            }
        }

        public static void DeleteIssuePages(this IDbConnection connection, IEnumerable<IssuePageDto> issuePages)
        {
            var sql = "Delete From IssuePage Where Id IN @Ids";
            connection.Execute(sql, new { Ids = issuePages.Select(f => f.Id) });
        }

        public static IssuePageDto GetIssuePageByNumber(this IDbConnection connection, int issueId, int sequenceNumber)
        {
            var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new { IssueId = issueId, SequenceNumber = sequenceNumber });

            return connection.QuerySingleOrDefault<IssuePageDto>(command);
        }

        public static IssuePageDto GetIssuePageByNumber(this IDbConnection connection, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            var sql = @"SELECT ip.*
                        FROM IssuePage ip
                        INNER JOIN Issue i ON i.Id = ip.Issueid
                        Where i.PeriodicalId = @PeriodicalId 
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber
            });

            return connection.QuerySingleOrDefault<IssuePageDto>(command);
        }

        public static IssuePageDto GetIssuePageByIssueId(this IDbConnection connection, int issueId, long pageId)
        {
            var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId AND Id = @Id";
            var command = new CommandDefinition(sql, new { IssueId = issueId, id = pageId });

            return connection.QuerySingleOrDefault<IssuePageDto>(command);
        }

        public static IEnumerable<IssuePageDto> GetIssuePagesByIssue(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT *
                        FROM IssuePage
                        Where IssueId = @IssueId";
            var command = new CommandDefinition(sql, new { IssueId = issueId });

            return connection.Query<IssuePageDto>(command);
        }
    }
}
