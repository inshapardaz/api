using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class IssueArticleDataHelper
    {
        public static void AddIssueArticle(this IDbConnection connection, IssueArticleDto issue)
        {
            var sql = @"Insert Into IssueArticle (Title, IssueId, SequenceNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SeriesName, SeriesIndex)
                        Output Inserted.Id
                        Values (@Title, @IssueId, @SequenceNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SeriesName, @SeriesIndex)";
            var id = connection.ExecuteScalar<int>(sql, issue);
            issue.Id = id;
        }

        public static void AddIssueArticles(this IDbConnection connection, IEnumerable<IssueArticleDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssueArticle(connection, issue);
            }
        }

        public static IssueArticleDto GetIssueArticleById(this IDbConnection connection, int articleId)
        {
            var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
            return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
        }

        public static IssueArticleDto GetIssueArticleById(this IDbConnection connection, long articleId)
        {
            var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
            return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
        }

        public static IssueArticleDto GetIssueArticlesByIssue(this IDbConnection connection, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.PeriodicalId = @PeriodicalId
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND a.SequenceNumber = @SequenceNumber";
            return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new {
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber
            });
        }

        public static void DeleteIssueArticles(this IDbConnection connection, IEnumerable<IssueArticleDto> articles)
        {
            var sql = "Delete From IssueArticle Where Id IN @Ids";
            connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
        }

        public static IEnumerable<IssueArticleDto> GetIssueArticlesByIssue(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.Id = @IssueId";
            return connection.Query<IssueArticleDto>(sql, new { IssueId = issueId });
        }

        public static void AddIssueArticleAuthor(this IDbConnection connection, int issueId, int authorId)
        {
            var sql = "INSERT INTO IssueArticleAuthor VALUES (@IssueId, @AuthorId)";
            connection.Execute(sql, new { IssueId = issueId, AuthorId = authorId });
        }

        public static IssueArticleContentDto GetIssueArticleContent(this IDbConnection connection, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language)
        {
            return connection.QuerySingleOrDefault<IssueArticleContentDto>(@"select ac.*
                    FROM IssueArticle a
                    INNER JOIN Issue i ON i.Id = a.IssueId
                    INNER JOIN Periodical p ON p.Id = i.Periodicalid
                    LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                    WHERE i.PeriodicalId = @PeriodicalId
                    AND i.VolumeNumber = @VolumeNumber
                    AND i.IssueNumber = @IssueNumber
                    AND a.SequenceNumber = @SequenceNumber
                    AND ac.Language = @Language", 
                new { 
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language
                });
        }

        public static IEnumerable<IssueArticleContentDto> GetIssueArticleContents(this IDbConnection connection, long articleId)
        {
            return connection.Query<IssueArticleContentDto>(@"select *
                    FROM IssueArticleContent
                    WHERE ArticleId = @Id",
                new
                {
                    Id = articleId
                });
        }

        public static IEnumerable<IssueArticleContentDto> GetContentByIssueArticle(this IDbConnection connection, int articleId)
        {
            return connection.Query<IssueArticleContentDto>("Select * From IssueArticleContent Where ArticleId = @Id", new { Id = articleId });
        }

        public static IEnumerable<IssueArticleContentDto> GetIssueContentByArticle(this IDbConnection connection, long articleId)
        {
            return connection.Query<IssueArticleContentDto>("Select * From IssueArticleContent Where ArticleId = @Id", new { Id = articleId });
        }

        public static void AddIssueArticleContents(this IDbConnection connection, IEnumerable<IssueArticleContentDto> contents)
        {
            var sql = "INSERT INTO IssueArticleContent (ArticleId, Language, Text) VALUES (@ArticleId, @Language, @Text)";
            connection.Execute(sql, contents);
        }

        public static void AddIssueArticleContents(this IDbConnection connection, IssueArticleContentDto content)
        {
            var sql = "INSERT INTO ArticleContent (ArticleId, Language, Text) VALUES (@ArticleId, @Language, @Text)";
            connection.Execute(sql, content);
        }

    }
}
