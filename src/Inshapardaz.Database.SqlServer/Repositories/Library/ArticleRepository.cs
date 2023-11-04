using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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

        public async Task<ArticleContentModel> AddArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into ArticleContent (ArticleId, Language, Text)
                            Values (@ArticleId, @Language, @Text)";
                var command = new CommandDefinition(sql, new { ArticleId = articleId, Language = language, Text = content }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetArticleContent(libraryId, articleId, language, cancellationToken);
            }
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

        public async Task DeleteArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE ac 
                            FROM ArticleContent ac
                            INNER JOIN Article a ON a.Id = ac.ArticleId
                            WHERE a.Id= @ArticleId
                                AND a.LibraryId = @LibraryId 
                                AND ac.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    ArticleId = articleId,
                    Language = language
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
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
                            WHERE a.LibraryId = @LibraryId AND ac.ArticleId = @ArticleId AND Language = @Language";
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

        public async Task<Page<ArticleModel>> GetArticles(int libraryId, string query, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";
                var sortByQuery = GetSortByQuery(sortBy, sortDirection, "at");
                var param = new
                {
                    LibraryId = libraryId,
                    Query = string.IsNullOrWhiteSpace(query) ? null : $"%{query}%",
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    AccountId = accountId,
                    AuthorFilter = filter.AuthorId,
                    TypeFilter = filter.Type == ArticleType.Unknown ? (ArticleType?) null : filter.Type,
                    CategoryFilter = filter.CategoryId,
                    FavoriteFilter = filter.Favorite,
                    RecentFilter = filter.Read,
                    StatusFilter = filter.Status 
                };

                var sql = @"SELECT at.Id
                            From Article at
                            INNER JOIN ArticleAuthor aa ON at.Id = aa.ArticleId
                            INNER JOIN Author a On aa.AuthorId = a.Id
                            LEFT JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                            LEFT JOIN Category c ON ac.CategoryId = c.Id
                            LEFT JOIN ArticleFavorite f On f.ArticleId= at.Id
                            LEFT JOIN ArticleRead r On at.Id = r.ArticleId
                            Where at.LibraryId = @LibraryId
                            AND (at.Title Like @Query OR @Query IS NULL)
                            AND (@AccountId IS NOT NULL OR at.IsPublic = 1)
                            AND (at.[Type] = @TypeFilter OR @TypeFilter = 0 OR @TypeFilter IS NULL)
                            AND (at.Status = @StatusFilter OR @StatusFilter = 0 OR @StatusFilter IS NULL)
                            AND (aa.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (ac.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            GROUP BY at.Id, at.Title, at.LastModified " +
                            $" ORDER BY {sortByQuery} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                var articleIds = await connection.QueryAsync(command);

                var sqlCount = @"SELECT Count(*) 
                                 FROM (
                                    SELECT at.Id
                                    FROM Article at
                                    INNER JOIN ArticleAuthor aa ON at.Id = aa.ArticleId
                                    INNER JOIN Author a On aa.AuthorId = a.Id
                                    LEFT JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                                    LEFT JOIN Category c ON ac.CategoryId = c.Id
                                    LEFT JOIN ArticleFavorite f On f.ArticleId= at.Id
                                    LEFT JOIN ArticleRead r On at.Id = r.ArticleId
                                    Where at.LibraryId = @LibraryId
                                    AND (at.Title Like @Query OR @Query IS NULL)
                                    AND (@AccountId IS NOT NULL OR at.IsPublic = 1)
                                    AND (at.[Type] = @TypeFilter OR @TypeFilter = 0 OR @TypeFilter IS NULL)
                                    AND (at.Status = @StatusFilter OR @StatusFilter = 0 OR @StatusFilter IS NULL)
                                    AND (aa.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                                    AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                                    AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                                    AND (ac.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                                    GROUP BY at.Id) as articleCounts";

                var articleCount = await connection.QuerySingleAsync<long>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

                var articles = await GetArticles(connection, libraryId, articleIds.Select(b => (long)b.Id).ToList(), accountId, cancellationToken);

                return new Page<ArticleModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = articleCount,
                    Data = articles
                };
            }
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

        public async Task<ArticleContentModel> UpdateArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE ac SET Text = @Text
                            FROM ArticleContent ac
                            INNER JOIN Article a ON a.Id = ac.ArticleId
                            WHERE a.Id= @ArticleId
                                AND a.LibraryId = @LibraryId 
                                AND ac.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    ArticleId = articleId,
                    Language = language,
                    Text = content
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetArticleContent(libraryId, articleId, language, cancellationToken);
            }
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
                
                var sql = @"SElect at.*, fl.FilePath AS ImageUrl,
                            CASE WHEN af.ArticleId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            a.*, c.*, con.*
                            FROM Article at
                            LEFT OUTER JOIN ArticleAuthor ara ON ara.ArticleId = at.Id
                            LEFT OUTER JOIN Author a ON ara.AuthorId = a.Id
                            LEFT OUTER JOIN ArticleFavorite af ON at.Id = af.ArticleId AND (af.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleRead ar On at.Id = ar.ArticleId AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                            LEFT OUTER JOIN Category c ON ac.CategoryId = c.Id
                            LEFT JOIN ArticleContent con ON con.ArticleId = at.Id
                            LEFT OUTER JOIN [File] fl ON fl.Id = at.ImageId
                            WhEre at.LibraryId = @LibraryId AND at.Id = @Id";
                await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleContentModel, ArticleModel>(sql, (ar, a, c, con) =>
                {
                    if (article == null)
                    {
                        article = ar;
                    }

                    if (a != null && !article.Authors.Any(x => x.Id == a.Id))
                    {
                        article.Authors.Add(a);
                    }

                    if (c != null && !article.Categories.Any(x => x.Id == c.Id))
                    {
                        article.Categories.Add(c);
                    }

                    if (con != null && !article.Contents.Any(x => x.Id == con.Id))
                    {
                        article.Contents.Add(con);
                    }

                    return article;

                }, new { LibraryId = libraryId, Id = articleId, AccountId = accountId });

                return article;
            }
        }

        private async Task<IEnumerable<ArticleModel>> GetArticles(IDbConnection connection, int libraryId, List<long> articleIds, int? accountId = null, CancellationToken cancellationToken = default)
        {
            var articles = new Dictionary<long, ArticleModel>();
            var sql3 = @"SELECT at.*, fl.FilePath AS ImageUrl,
                            CASE WHEN af.ArticleId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            a.*, c.*, con.*
                        FROM Article at
                            LEFT OUTER JOIN ArticleAuthor ara ON ara.ArticleId = at.Id
                            LEFT OUTER JOIN Author a On ara.AuthorId = a.Id
                            LEFT OUTER JOIN ArticleFavorite af On at.Id = af.ArticleId 
                                AND (af.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleRead ar On at.Id = ar.ArticleId 
                                AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                            LEFT OUTER JOIN Category c ON ac.CategoryId = c.Id
                            LEFT OUTER JOIN [File] fl ON fl.Id = at.ImageId
                            LEFT JOIN ArticleContent con ON con.ArticleId = at.Id
                        WHERE at.LibraryId = @LibraryId 
                            AND at.Id IN @ArticleList";
            var command3 = new CommandDefinition(sql3, new { 
                LibraryId = libraryId, 
                ArticleList = articleIds,
                AccountId = accountId
            }, cancellationToken: cancellationToken);

            await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleContentModel, ArticleModel>(command3, (at, a, c, con) =>
            {
                if (!articles.TryGetValue(at.Id, out ArticleModel article))
                    articles.Add(at.Id, article = at);

                if (!article.Authors.Any(x => x.Id == a.Id))
                {
                    article.Authors.Add(a);
                }

                if (c != null && !article.Categories.Any(x => x.Id == c.Id))
                {
                    article.Categories.Add(c);
                }

                if (con != null && !article.Contents.Any(x => x.Id == con.Id))
                {
                    article.Contents.Add(con);
                }

                return article;
            });

            return articles.Values.OrderBy(b => articleIds.IndexOf(b.Id)).ToList();
        }

        private static string GetSortByQuery(ArticleSortByType sortBy, string sortDiretion, string prefix = "")
        {
            switch (sortBy)
            {
                case ArticleSortByType.LastModified:
                    return $"{prefix}.LastModified {sortDiretion}, {prefix}.Title {sortDiretion}";

                case ArticleSortByType.Title:
                default:
                    return $"{prefix}.Title {sortDiretion}";
            }
        }

        public async Task UpdateArticleImage(int libraryId, long articleId, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update Article
                            Set ImageId = @ImageId
                            Where Id = @ArticleId And LibraryId = @LibraryId;";
                var command = new CommandDefinition(sql, new { ImageId = imageId, ArticleId = articleId, LibraryId = libraryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }


        public async Task AddArticleToFavorites(int libraryId, int? accountId, long articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO ArticleFavorite (LibraryId, ArticleId, AccountId) VALUES (@LibraryId, @ArticleId, @AccountId)";
                var command = new CommandDefinition(sql, new { 
                    LibraryId = libraryId, 
                    ArticleId = articleId, 
                    AccountId = accountId }, 
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task RemoveArticleFromFavorites(int libraryId, int? accountId, long articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE FROM ArticleFavorite WHERE LibraryId = @Libraryid AND ArticleId = @ArticleId AND AccountId = @AccountId";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    ArticleId = articleId,
                    AccountId = accountId
                },
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
