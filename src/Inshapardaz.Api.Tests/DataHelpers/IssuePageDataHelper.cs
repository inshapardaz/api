using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class IssuePageDataHelper
    {
        public static void AddIssuePage(this IDbConnection connection, IssuePageDto issuePage)
        {
            var sql = @"Insert Into IssuePage (IssueId, Text, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                        Output Inserted.Id
                        Values (@IssueId, @Text, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status)";
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
            var command = new CommandDefinition(sql, new {
                PeriodicalId = periodicalId, 
                VolumeNumber = volumeNumber, 
                IssueNumber = issueNumber, 
                SequenceNumber = sequenceNumber });

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
