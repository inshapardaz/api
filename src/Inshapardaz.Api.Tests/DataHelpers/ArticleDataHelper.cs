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
        public static void AddArticle(this IDbConnection connection, ArticleDto article)
        {
            var sql = @"Insert Into Article (LibraryId, Title, IsPublic, ImageId, Type, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SourceId, SourceType)
                        Output Inserted.Id
                        Values (@LibraryId, @Title, @IsPublic, @ImageId, @Type, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SourceId, @SourceType)";
            var id = connection.ExecuteScalar<int>(sql, article);
            article.Id = id;
        }

        public static void AddArticles(this IDbConnection connection, IEnumerable<ArticleDto> issues)
        {
            foreach (var issue in issues)
            {
                AddArticle(connection, issue);
            }
        }

        public static ArticleDto GetArticleById(this IDbConnection connection, long articleId)
        {
            var sql = @"SELECT * FROM Article WHERE Id = @Id";
            return connection.QuerySingleOrDefault<ArticleDto>(sql, new { Id = articleId });
        }

        public static void DeleteArticles(this IDbConnection connection, IEnumerable<ArticleDto> articles)
        {
            var sql = "Delete From Article Where Id IN @Ids";
            connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
        }

        public static void AddArticleAuthor(this IDbConnection connection, long articleId, int authorId)
        {
            var sql = "INSERT INTO ArticleAuthor VALUES (@ArticleId, @AuthorId)";
            connection.Execute(sql, new { ArticleId = articleId, AuthorId = authorId });
        }

        public static IssueArticleContentDto GetArticleContent(this IDbConnection connection, long articleId, string language)
        {
            return connection.QuerySingleOrDefault<IssueArticleContentDto>(@"select ac.*
                    FROM Article a
                    LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                    WHERE a.articleId = @ArticleId
                    AND ac.Language = @Language",
                new
                {
                    ArticleId = articleId,
                    Language = language
                });
        }

        public static IEnumerable<ArticleContentDto> GetArticleContents(this IDbConnection connection, long articleId)
        {
            return connection.Query<ArticleContentDto>(@"select *
                    FROM ArticleContent
                    WHERE ArticleId = @Id",
                new
                {
                    Id = articleId
                });
        }

        public static IEnumerable<IssueArticleContentDto> GetContentByArticle(this IDbConnection connection, long articleId)
        {
            return connection.Query<IssueArticleContentDto>("Select * From ArticleContent Where ArticleId = @Id", new { Id = articleId });
        }

        public static int AddArticleContents(this IDbConnection connection, ArticleContentDto content)
        {
            var sql = @"INSERT INTO ArticleContent (ArticleId, Language, Text)
                OUTPUT Inserted.ID
                VALUES (@ArticleId, @Language, @Text)";
            return connection.ExecuteScalar<int>(sql, content);
        }
        public static void AddArticlesToFavorites(this IDbConnection connection, int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            articleIds.ForEach(id => connection.AddArticleToFavorites(libraryId, id, accountId));
        }


        public static void AddArticleToFavorites(this IDbConnection connection, int libraryId, long articleId, int accountId, DateTime? timestamp = null)
        {
            var sql = @"Insert into ArticleFavorite (LibraryId, ArticleId, AccountId)
                        Values (@LibraryId, @ArticleId, @AccountId)";
            connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, ArticleId = articleId, AccountId = accountId });
        }


        public static bool DoesArticleExistsInFavorites(this IDbConnection connection, long articleId, int accountId) =>
           connection.QuerySingle<bool>(@"Select Count(1) From ArticleFavorite Where ArticleId = @ArticleId And AccountId = @AccountId", new
           {
               ArticleId = articleId,
               AccountId = accountId
           });


        public static void AddArticlesToRecentReads(this IDbConnection connection, int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            articleIds.ForEach(id => connection.AddArticleToRecentReads(new RecentArticleDto { LibraryId = libraryId, ArticleId = id, AccountId = accountId, DateRead = DateTime.UtcNow }));
        }

        public static void AddArticleToRecentReads(this IDbConnection connection, RecentArticleDto dto)
        {
            var sql = @"Insert into ArticleRead (LibraryId, ArticleId, AccountId, DateRead)
                        Values (@LibraryId, @ArticleId, @AccountId, @DateRead)";
            connection.ExecuteScalar<int>(sql, dto);
        }


        public static bool DoesArticleExistsInRecent(this IDbConnection connection, long articleId) =>
            connection.QuerySingle<bool>(@"Select Count(1) From ArticleRead Where ArticleId = @ArticleId", new
            {
                ArticleId = articleId
            });
    }
}
