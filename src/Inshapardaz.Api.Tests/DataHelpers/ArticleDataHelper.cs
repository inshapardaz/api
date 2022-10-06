using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class ArticleDataHelper
    {
        public static void AddArticle(this IDbConnection connection, ArticleDto issue)
        {
            var sql = @"Insert Into Article (Title, IssueId, SequenceNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SeriesName, SeriesIndex)
                        Output Inserted.Id
                        Values (@Title, @IssueId, @SequenceNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SeriesName, @SeriesIndex)";
            var id = connection.ExecuteScalar<int>(sql, issue);
            issue.Id = id;
        }

        public static void AddArticles(this IDbConnection connection, IEnumerable<ArticleDto> issues)
        {
            foreach (var issue in issues)
            {
                AddArticle(connection, issue);
            }
        }

        public static ArticleDto GetArticleById(this IDbConnection connection, int articleId)
        {
            var sql = @"SELECT * FROM Article WHERE Id = @Id";
            return connection.QuerySingleOrDefault<ArticleDto>(sql, new { Id = articleId });
        }

        public static ArticleDto GetArticlesByIssue(this IDbConnection connection, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            var sql = @"SELECT a.* FROM Article a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.PeriodicalId = @PeriodicalId
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND a.SequenceNumber = @SequenceNumber";
            return connection.QuerySingleOrDefault<ArticleDto>(sql, new {
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber
            });
        }

        public static void DeleteArticles(this IDbConnection connection, IEnumerable<ArticleDto> articles)
        {
            var sql = "Delete From Article Where Id IN @Ids";
            connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
        }

        public static IEnumerable<ArticleDto> GetArticlesByIssue(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT a.* FROM Article a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.Id = @IssueId";
            return connection.Query<ArticleDto>(sql, new { IssueId = issueId });
        }

        public static void AddArticleAuthor(this IDbConnection connection, int issueId, int authorId)
        {
            var sql = "INSERT INTO ArticleAuthor VALUES (@IssueId, @AuthorId)";
            connection.Execute(sql, new { IssueId = issueId, AuthorId = authorId });
        }

        public static ArticleContentDto GetArticleContentById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<ArticleContentDto>("Select * From ArticleContent Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<ArticleContentDto> GetContentByArticle(this IDbConnection connection, int articleId)
        {
            return connection.Query<ArticleContentDto>("Select * From ArticleContent Where ArticleId = @Id", new { Id = articleId });
        }

        public static void AddArticleContents(this IDbConnection connection, IEnumerable<ArticleContentDto> contents)
        {
            var sql = "INSERT INTO ArticleContent (ArticleId, Language, Text) VALUES (@ArticleId, @Language, @Text)";
            connection.Execute(sql, contents);
        }

        public static void AddArticleContents(this IDbConnection connection, ArticleContentDto content)
        {
            var sql = "INSERT INTO ArticleContent (ArticleId, Language, Text) VALUES (@ArticleId, @Language, @Text)";
            connection.Execute(sql, content);
        }

    }
}
