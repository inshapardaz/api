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
            var sql = @"Insert Into Article (Title, IssueId, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp)
                        Output Inserted.Id
                        Values (@Title, @IssueId, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)";
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
    }
}
