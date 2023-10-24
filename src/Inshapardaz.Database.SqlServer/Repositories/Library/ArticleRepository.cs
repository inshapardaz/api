using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public ArticleRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, int? accountId, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO Article (LibraryId, Title, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                            OUTPUT Inserted.Id VALUES (@LibraryId, @Title, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    Title = article.Title,
                    Status = article.Status,
                    WriterAccountId = article.WriterAccountId,
                    WriterAssignTimeStamp = article.WriterAssignTimeStamp,
                    ReviewerAccountId = article.ReviewerAccountId,
                    ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp
                }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);

                if (article.Authors != null && article.Authors.Any())
                {
                    var sqlAuthor = @"Insert Into ArticleAuthor (ArticleId, AuthorId) Values (@ArticleId, @AuthorId);";
                    var articleAuthors = article.Authors.Select(a => new { ArticleId = id, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, articleAuthors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }

                if (article.Categories != null && article.Categories.Any())
                {
                    var sqlCategory = @"Insert Into ArticleCategory (ArticleId, CategoryId) Values (@ArticleId, @CategoryId);";
                    var articleCategories = article.Categories.Select(c => new { ArticleId = id, CategoryId = c.Id });
                    var commandCategory = new CommandDefinition(sqlCategory, articleCategories, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            return await GetArticleById(libraryId, id, accountId, cancellationToken);
        }

        public Task<ArticleContentModel> AddArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteArticle(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Delete From Article Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = articleId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public Task DeleteArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> GetArticle(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            return GetArticleById(libraryId, articleId, null, cancellationToken);
        }

        public async Task<ArticleContentModel> GetArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ac.*
                            FROM ArticleContent ac
                            INNER JOIN Article a ON a.Id = ac.ArticleId
                            WHERE b.LibraryId = @LibraryId AND ac.ArticleId = @ArticleId AND Language = @Language";
                var command = new CommandDefinition(sql, new { 
                    LibraryId = libraryId, 
                    ArticleId = articleId,
                    Language = language
                }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ArticleContentModel>(command);
            }
        }

        public async Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ac.*
                            FROM ArticleContent ac
                            INNER JOIN Article a ON a.Id = ac.ArticleId
                            WHERE a.LibraryId = @LibraryId AND ac.ArticleId = @ArticleId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, ArticleId = articleId }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<ArticleContentModel>(command);
            }
        }

        public Task<Page<ArticleModel>> GetArticles(int libraryId, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Page<ArticleModel>> SearchArticles(int libraryId, string query, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ArticleModel> UpdateArticle(int libraryId, long articleId, ArticleModel article, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update Article SET
                            Title = @Title,
                            IsPublic = @IsPublic,
                            [Status] = @Status,
                            [Type] = @Type,
                            WriterAccountId = @WriterAccountId,
                            WriterAssignTimeStamp = @WriterAssignTimeStamp,
                            ReviewerAccountId = @ReviewerAccountId,
                            ReviewerAssignTimeStamp = @ReviewerAssignTimeStamp,
                            SourceType = @SourceType,
                            SourceId = @SourceId,
                            LastModified = GETDATE()
                            Where LibraryId = @LibraryId And Id = @Id";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    Id = article.Id,
                    Title = article.Title,
                    IsPublic = article.IsPublic,
                    Status = article.Status,
                    Type = article.Type,
                    WriterAccountId = article.WriterAccountId,
                    WriterAssignTimeStamp = article.WriterAssignTimeStamp,
                    ReviewerAccountId = article.ReviewerAccountId,
                    ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp,
                    SourceType = article.SourceType,
                    SourceId = article.SourceId,
                }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                await connection.ExecuteAsync(new CommandDefinition(
                                    "Delete From ArticleAuthor Where ArticleId = @ArticleId",
                                    new { ArticleId = article.Id },
                                    cancellationToken: cancellationToken));

                var sqlAuthor = @"Insert Into ArticleAuthor (ArticleId, AuthorId) Values (@ArticleId, @AuthorId);";

                if (article.Authors != null && article.Authors.Any())
                {
                    var authors = article.Authors.Select(a => new { ArticleId = article.Id, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, authors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }

                await connection.ExecuteAsync(new CommandDefinition(
                    "Delete From ArticleCategory Where ArticleId = @ArticleId",
                    new { ArticleId = article.Id },
                    cancellationToken: cancellationToken));

                var sqlCategory = @"Insert Into ArticleCategory (ArticleId, CategoryId) Values (@ArticleId, @CategoryId);";

                if (article.Categories != null && article.Categories.Any())
                {
                    var categories = article.Categories.Select(c => new { ArticleId = article.Id, CategoryId = c.Id });
                    var commandCategory = new CommandDefinition(sqlCategory, categories, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            return await GetArticleById(libraryId, articleId, null, cancellationToken);  
        }

        public Task<ArticleContentModel> UpdateArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> UpdateReviewerAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> UpdateWriterAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private async Task<ArticleModel> GetArticleById(int libraryId, long articleId,  int? accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                ArticleModel article = null;
                
                var sql = @"Select at.*, fl.FilePath AS ImageUrl,
                            CASE WHEN af.ArticleId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            a.*, c.*
                            from Article at
                            Left Outer Join ArticleAuthor ara ON ara.ArticleId = at.Id
                            Left Outer Join Author a On ara.AuthorId = a.Id
                            Left Outer Join ArticleFavorite af On at.Id = af.ArticleId AND (af.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join ArticleRead ar On at.Id = ar.ArticleId AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join ArticleCategory ac ON at.Id = ac.ArticleId
                            Left Outer Join Category c ON ac.CategoryId = c.Id
                            LEFT OUTER JOIN [File] fl ON fl.Id = at.ImageId
                            Where at.LibraryId = @LibraryId AND at.Id = @Id";
                await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleModel>(sql, (ar, a, c) =>
                {
                    if (article == null)
                    {
                        article = ar;
                    }

                    if (!article.Authors.Any(x => x.Id == a.Id))
                    {
                        article.Authors.Add(a);
                    }

                    if (!article.Categories.Any(x => x.Id == c.Id))
                    {
                        article.Categories.Add(c);
                    }
                    return article;

                }, new { LibraryId = libraryId, Id = articleId, AccountId = accountId });

                return article;
            }
        }
    }
}
