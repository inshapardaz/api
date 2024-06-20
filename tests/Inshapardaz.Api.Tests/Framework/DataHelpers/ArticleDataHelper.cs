using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IArticleTestRepository
    {
        void AddArticle(ArticleDto article);

        void AddArticles(IEnumerable<ArticleDto> articles);

        ArticleDto GetArticleById(long articleId);

        void DeleteArticles(IEnumerable<ArticleDto> articles);

        void AddArticleAuthor(long articleId, int authorId);

        ArticleContentDto GetArticleContent(long articleId, string language);

        IEnumerable<ArticleContentDto> GetArticleContents(long articleId);

        IEnumerable<IssueArticleContentDto> GetContentByArticle(long articleId);

        int AddArticleContents(ArticleContentDto content);
        void AddArticlesToFavorites(int libraryId, IEnumerable<long> articleIds, int accountId);

        void AddArticleToFavorites(int libraryId, long articleId, int accountId, DateTime? timestamp = null);


        bool DoesArticleExistsInFavorites(long articleId, int accountId);


        void AddArticlesToRecentReads(int libraryId, IEnumerable<long> articleIds, int accountId);

        void AddArticleToRecentReads(RecentArticleDto dto);


        bool DoesArticleExistsInRecent(long articleId);

        string GetArticleImageUrl(long articleId);

        FileDto GetArticleImage(long articleId);
        int GetArticleCountByAuthor(int id);
    }

    public class MySqlArticleTestRepository : IArticleTestRepository
    {

        private IProvideConnection _connectionProvider;

        public MySqlArticleTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddArticle(ArticleDto article)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Article (LibraryId, `Title`, IsPublic, ImageId, `Type`, `Status`, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SourceId, SourceType, LastModified)
                        Values (@LibraryId, @Title, @IsPublic, @ImageId, @Type, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SourceId, @SourceType, @LastModified);
                    SELECT LAST_INSERT_ID()";
                var id = connection.ExecuteScalar<int>(sql, article);
                article.Id = id;
            }
        }

        public void AddArticles(IEnumerable<ArticleDto> articles)
        {
            foreach (var article in articles)
            {
                AddArticle(article);
            }
        }

        public ArticleDto GetArticleById(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Article WHERE Id = @Id";
                return connection.QuerySingleOrDefault<ArticleDto>(sql, new { Id = articleId });
            }
        }

        public void DeleteArticles(IEnumerable<ArticleDto> articles)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROm Article WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
            }
        }

        public void AddArticleAuthor(long articleId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO ArticleAuthor VALUES (@ArticleId, @AuthorId)";
                connection.Execute(sql, new { ArticleId = articleId, AuthorId = authorId });
            }
        }

        public ArticleContentDto GetArticleContent(long articleId, string language)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<ArticleContentDto>(@"SELECT ac.*
                    FROM Article a
                    LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                    WHERE a.Id = @ArticleId
                    AND ac.Language = @Language",
                    new
                    {
                        ArticleId = articleId,
                        Language = language
                    });
            }
        }

        public IEnumerable<ArticleContentDto> GetArticleContents(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<ArticleContentDto>(@"SELECT *
                    FROM ArticleContent
                    WHERE ArticleId = @Id",
                    new
                    {
                        Id = articleId
                    });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetContentByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM ArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public int AddArticleContents(ArticleContentDto content)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleContent (ArticleId, `Language`, FileId, `Layout`)
                VALUES (@ArticleId, @Language, @FileId, @Layout);
                SELECT LAST_INSERT_ID();";
                return connection.ExecuteScalar<int>(sql, content);
            }
        }
        public void AddArticlesToFavorites(int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                articleIds.ForEach(id => AddArticleToFavorites(libraryId, id, accountId));
            }
        }

        public void AddArticleToFavorites(int libraryId, long articleId, int accountId, DateTime? timestamp = null)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleFavorite (LibraryId, ArticleId, AccountId)
                        VALUES (@LibraryId, @ArticleId, @AccountId)";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, ArticleId = articleId, AccountId = accountId });
            }
        }

        public bool DoesArticleExistsInFavorites(long articleId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM ArticleFavorite WHERE ArticleId = @ArticleId AND AccountId = @AccountId", new
                {
                    ArticleId = articleId,
                    AccountId = accountId
                });
            }
        }

        public void AddArticlesToRecentReads(int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                articleIds.ForEach(id => AddArticleToRecentReads(new RecentArticleDto { LibraryId = libraryId, ArticleId = id, AccountId = accountId, DateRead = DateTime.UtcNow }));
            }
        }

        public void AddArticleToRecentReads(RecentArticleDto dto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleRead (LibraryId, ArticleId, AccountId, DateRead)
                                VALUES (@LibraryId, @ArticleId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, dto);
            }
        }


        public bool DoesArticleExistsInRecent(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM ArticleRead WHERE ArticleId = @ArticleId", new
                {
                    ArticleId = articleId
                });
            }
        }

        public string GetArticleImageUrl(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {

                var sql = @"SELECT f.FilePath FROM `File` f
                                INNER JOIN Article a ON f.Id = a.ImageId
                                WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = articleId });
            }
        }

        public FileDto GetArticleImage(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* FROM `File` f
                                INNER JOIN Article a ON f.Id = a.ImageId
                                WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = articleId });
            }
        }

        public int GetArticleCountByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT Count(*) FROM ArticleAuthor WHERE AuthorId = @Id";
                return connection.ExecuteScalar<int>(sql, new { Id = id });
            }
        }

    }

    public class SqlServerArticleTestRepository : IArticleTestRepository
    {

        private IProvideConnection _connectionProvider;

        public SqlServerArticleTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddArticle(ArticleDto article)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Article (LibraryId, Title, IsPublic, ImageId, Type, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SourceId, SourceType, LastModified)
                        OUTPUT INSERTED.ID
                        VALUES (@LibraryId, @Title, @IsPublic, @ImageId, @Type, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SourceId, @SourceType, @LastModified)";
                var id = connection.ExecuteScalar<int>(sql, article);
                article.Id = id;
            }
        }

        public void AddArticles(IEnumerable<ArticleDto> articles)
        {
            foreach (var article in articles)
            {
                AddArticle(article);
            }
        }

        public ArticleDto GetArticleById(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Article WHERE Id = @Id";
                return connection.QuerySingleOrDefault<ArticleDto>(sql, new { Id = articleId });
            }
        }

        public void DeleteArticles(IEnumerable<ArticleDto> articles)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROm Article WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
            }
        }

        public void AddArticleAuthor(long articleId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO ArticleAuthor VALUES (@ArticleId, @AuthorId)";
                connection.Execute(sql, new { ArticleId = articleId, AuthorId = authorId });
            }
        }

        public ArticleContentDto GetArticleContent(long articleId, string language)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<ArticleContentDto>(@"SELECT ac.*
                    FROM Article a
                    LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                    WHERE a.Id = @ArticleId
                    AND ac.Language = @Language",
                    new
                    {
                        ArticleId = articleId,
                        Language = language
                    });
            }
        }

        public IEnumerable<ArticleContentDto> GetArticleContents(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<ArticleContentDto>(@"SELECT *
                    FROM ArticleContent
                    WHERE ArticleId = @Id",
                    new
                    {
                        Id = articleId
                    });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetContentByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM ArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public int AddArticleContents(ArticleContentDto content)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleContent (ArticleId, Language, FileId, Layout)
                OUTPUT Inserted.ID
                VALUES (@ArticleId, @Language, @FileId, @Layout)";
                return connection.ExecuteScalar<int>(sql, content);
            }
        }
        public void AddArticlesToFavorites(int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                articleIds.ForEach(id => AddArticleToFavorites(libraryId, id, accountId));
            }
        }

        public void AddArticleToFavorites(int libraryId, long articleId, int accountId, DateTime? timestamp = null)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleFavorite (LibraryId, ArticleId, AccountId)
                        VALUES (@LibraryId, @ArticleId, @AccountId)";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, ArticleId = articleId, AccountId = accountId });
            }
        }


        public bool DoesArticleExistsInFavorites(long articleId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM ArticleFavorite WHERE ArticleId = @ArticleId AND AccountId = @AccountId", new
                {
                    ArticleId = articleId,
                    AccountId = accountId
                });
            }
        }

        public void AddArticlesToRecentReads(int libraryId, IEnumerable<long> articleIds, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                articleIds.ForEach(id => AddArticleToRecentReads(new RecentArticleDto { LibraryId = libraryId, ArticleId = id, AccountId = accountId, DateRead = DateTime.UtcNow }));
            }
        }

        public void AddArticleToRecentReads(RecentArticleDto dto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO ArticleRead (LibraryId, ArticleId, AccountId, DateRead)
                                VALUES (@LibraryId, @ArticleId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, dto);
            }
        }

        public bool DoesArticleExistsInRecent(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM ArticleRead WHERE ArticleId = @ArticleId", new
                {
                    ArticleId = articleId
                });
            }
        }
        public string GetArticleImageUrl(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM [File] f
                                INNER JOIN Article a ON f.Id = a.ImageId
                                WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = articleId });
            }
        }

        public FileDto GetArticleImage(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* FROM [File] f
                                INNER JOIN Article a ON f.Id = a.ImageId
                                WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = articleId });
            }
        }

        public int GetArticleCountByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT Count(*) FROM ArticleAuthor WHERE AuthorId = @Id";
                return connection.ExecuteScalar<int>(sql, new { Id = id });
            }
        }

    }
}
