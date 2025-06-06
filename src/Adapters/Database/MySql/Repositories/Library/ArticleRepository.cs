using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class ArticleRepository : IArticleRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public ArticleRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
    public async Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, int? accountId, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO Article (LibraryId, `Title`, `Status`, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, `Type`) 
                            VALUES (@LibraryId, @Title, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Type);
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                Title = article.Title,
                Status = article.Status,
                Type = article.Type,
                WriterAccountId = article.WriterAccountId,
                WriterAssignTimeStamp = article.WriterAssignTimeStamp,
                ReviewerAccountId = article.ReviewerAccountId,
                ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp,
            }, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);

            if (article.Authors != null && article.Authors.Any())
            {
                var sqlAuthor = @"INSERT INTO ArticleAuthor (ArticleId, AuthorId) VALUES (@ArticleId, @AuthorId);";
                var articleAuthors = article.Authors.Select(a => new { ArticleId = id, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, articleAuthors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            if (article.Categories != null && article.Categories.Any())
            {
                var sqlCategory = @"INSERT INTO ArticleCategory (ArticleId, CategoryId) VALUES (@ArticleId, @CategoryId);";
                var articleCategories = article.Categories.Select(c => new { ArticleId = id, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, articleCategories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }
            
            if (article.Tags != null && article.Tags.Any())
            {
                foreach (var tag in article.Tags)
                {
                    var tagId = await connection.ExecuteScalarAsync<int>(
                        new CommandDefinition(
                            @"INSERT INTO Tag (Name, LibraryId) 
                              VALUES (@Name, @LibraryId) 
                              ON DUPLICATE KEY UPDATE Name=@Name; 
                              SELECT Id FROM Tag WHERE Name = @Name AND  LibraryId = @LibraryId;",
                            new { Name = tag.Name, LibraryId = libraryId },
                            cancellationToken: cancellationToken));
            
                    // Associate tag with book
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            "INSERT INTO ArticleTag (ArticleId, TagId) VALUES (@ArticleId, @TagId);",
                            new { ArticleId = id, TagId = tagId },
                            cancellationToken: cancellationToken));
                }
            }
        }

        return await GetArticleById(libraryId, id, accountId, cancellationToken);
    }

    public async Task<ArticleContentModel> AddArticleContent(int libraryId, ArticleContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Insert Into ArticleContent (ArticleId, `Language`, `Text`, FileId , Layout)
                            VALUES (@ArticleId, @Language, @Text, @FileId, @Layout)";
            var command = new CommandDefinition(sql, content, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetArticleContent(libraryId, content.ArticleId, content.Language, cancellationToken);
        }
    }

    public async Task DeleteArticle(int libraryId, long articleId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM Article WHERE LibraryId = @LibraryId AND Id = @Id";
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
                                AND ac.`Language` = @Language";
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
                            WHERE a.LibraryId = @LibraryId 
                                AND ac.ArticleId = @ArticleId 
                                AND `Language` = @Language";
            var command = new CommandDefinition(sql, new
            {
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
                            WHERE a.LibraryId = @LibraryId 
                                AND ac.ArticleId = @ArticleId";
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
                Offset = pageSize * (pageNumber - 1),
                AccountId = accountId,
                AuthorFilter = filter.AuthorId,
                TypeFilter = filter.Type == ArticleType.Unknown ? (ArticleType?)null : filter.Type,
                CategoryFilter = filter.CategoryId,
                TagFilter = filter.TagId,
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
                            LEFT OUTER JOIN ArticleTag att ON at.Id = att.ArticleId
                            LEFT OUTER JOIN Tag t ON att.TagId = t.Id
                            LEFT JOIN ArticleFavorite f On f.ArticleId= at.Id
                            LEFT JOIN ArticleRead r On at.Id = r.ArticleId
                        WHERE at.LibraryId = @LibraryId
                            AND (at.Title Like @Query OR @Query IS NULL)
                            AND (@AccountId IS NOT NULL OR at.IsPublic = 1)
                            AND (at.`Type` = @TypeFilter OR @TypeFilter = 0 OR @TypeFilter IS NULL)
                            AND (at.`Status` = @StatusFilter OR @StatusFilter = 0 OR @StatusFilter IS NULL)
                            AND (aa.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (ac.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (att.TagId = @TagFilter OR @TagFilter IS NULL)
                            GROUP BY at.Id, at.Title, at.LastModified " +
                        $" ORDER BY {sortByQuery} " +
                        @"LIMIT @PageSize 
                            OFFSET @Offset";

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
                                        LEFT OUTER JOIN ArticleTag att ON at.Id = att.ArticleId
                                        LEFT OUTER JOIN Tag t ON att.TagId = t.Id
                                        LEFT JOIN ArticleFavorite f On f.ArticleId= at.Id
                                        LEFT JOIN ArticleRead r On at.Id = r.ArticleId
                                    WHERE at.LibraryId = @LibraryId
                                        AND (at.Title Like @Query OR @Query IS NULL)
                                        AND (@AccountId IS NOT NULL OR at.IsPublic = 1)
                                        AND (at.`Type` = @TypeFilter OR @TypeFilter = 0 OR @TypeFilter IS NULL)
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

    public async Task<ArticleModel> UpdateArticle(int libraryId, ArticleModel article, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Article SET
                            `Title` = @Title,
                            IsPublic = @IsPublic,
                            `Status` = @Status,
                            `Type` = @Type,
                            WriterAccountId = @WriterAccountId,
                            WriterAssignTimeStamp = @WriterAssignTimeStamp,
                            ReviewerAccountId = @ReviewerAccountId,
                            ReviewerAssignTimeStamp = @ReviewerAssignTimeStamp,
                            SourceType = @SourceType,
                            SourceId = @SourceId,
                            LastModified = UTC_TIMESTAMP()
                            Where LibraryId = @LibraryId AND Id = @Id";
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
                                "DELETE FROM ArticleAuthor WHERE ArticleId = @ArticleId",
                                new { ArticleId = article.Id },
                                cancellationToken: cancellationToken));

            var sqlAuthor = @"INSERT INTO ArticleAuthor (ArticleId, AuthorId) VALUES (@ArticleId, @AuthorId);";

            if (article.Authors != null && article.Authors.Any())
            {
                var authors = article.Authors.Select(a => new { ArticleId = article.Id, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, authors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM ArticleCategory WHERE ArticleId = @ArticleId",
                new { ArticleId = article.Id },
                cancellationToken: cancellationToken));

            var sqlCategory = @"INSERT INTO ArticleCategory (ArticleId, CategoryId) VALUES (@ArticleId, @CategoryId);";

            if (article.Categories != null && article.Categories.Any())
            {
                var categories = article.Categories.Select(c => new { ArticleId = article.Id, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, categories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }
            
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM ArticleTag WHERE ArticleId = @ArticleId",
                new { ArticleId = article.Id },
                cancellationToken: cancellationToken));
            
            if (article.Tags != null && article.Tags.Any())
            {
                foreach (var tag in article.Tags)
                {
                    var tagId = await connection.ExecuteScalarAsync<int>(
                        new CommandDefinition(
                            @"INSERT INTO Tag (Name, LibraryId) 
                              VALUES (@Name, @LibraryId) 
                              ON DUPLICATE KEY UPDATE Name=@Name; 
                              SELECT Id FROM Tag WHERE Name = @Name AND  LibraryId = @LibraryId;",
                            new { Name = tag.Name, LibraryId = libraryId },
                            cancellationToken: cancellationToken));
            
                    // Associate tag with book
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            "INSERT INTO ArticleTag (ArticleId, TagId) VALUES (@ArticleId, @TagId);",
                            new { ArticleId = article.Id, TagId = tagId },
                            cancellationToken: cancellationToken));
                }
            }
        }

        return await GetArticleById(libraryId, article.Id, null, cancellationToken);
    }

    public async Task<ArticleContentModel> UpdateArticleContent(int libraryId, ArticleContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE ArticleContent ac
                                INNER JOIN Article a ON a.Id = ac.ArticleId 
                            SET 
                                FileId = @FileId,
                                Layout = @Layout
                            WHERE a.Id= @ArticleId
                                AND a.LibraryId = @LibraryId 
                                AND ac.`Language` = @Language";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                ArticleId = content.ArticleId,
                Language = content.Language,
                FileId = content.FileId,
                Layout = content.Layout
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetArticleContent(libraryId, content.ArticleId, content.Language, cancellationToken);
        }
    }

    public async Task<ArticleModel> UpdateReviewerAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Article
                            SET ReviewerAccountId = @ReviewerAccountId, ReviewerAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE LibraryId = @LibraryId AND Id = @ArticleId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                ReviewerAccountId = accountId,
                ArticleId = articleId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetArticle(libraryId, articleId, cancellationToken);
        }
    }

    public async Task<ArticleModel> UpdateWriterAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPdate Article
                            SET WriterAccountId = @WriterAccountId, WriterAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE LibraryId = @LibraryId AND Id = @ArticleId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                WriterAccountId = accountId,
                ArticleId = articleId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetArticle(libraryId, articleId, cancellationToken);
        }
    }

    private async Task<ArticleModel> GetArticleById(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            ArticleModel article = null;

            var sql = @"SElect at.*, fl.FilePath AS ImageUrl,
                            CASE WHEN af.ArticleId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            aw.Name As WriterAccountName,
                            arv.Name As ReviewerAccountName,
                            a.*, c.*, con.*, aw.*, arv.*, t.*
                            FROM Article at
                                LEFT OUTER JOIN ArticleAuthor ara ON ara.ArticleId = at.Id
                                LEFT OUTER JOIN Author a ON ara.AuthorId = a.Id
                                LEFT OUTER JOIN ArticleFavorite af ON at.Id = af.ArticleId AND (af.AccountId = @AccountId OR @AccountId Is Null)
                                LEFT OUTER JOIN ArticleRead ar On at.Id = ar.ArticleId AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                                LEFT OUTER JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                                LEFT OUTER JOIN Category c ON ac.CategoryId = c.Id
                                LEFT OUTER JOIN ArticleTag att ON at.Id = att.ArticleId
                                LEFT OUTER JOIN Tag t ON att.TagId = t.Id
                                LEFT JOIN ArticleContent con ON con.ArticleId = at.Id
                                LEFT OUTER JOIN Accounts aw ON aw.Id = WriterAccountId
                                LEFT OUTER JOIN Accounts arv ON arv.Id = ReviewerAccountId
                                LEFT OUTER JOIN `File` fl ON fl.Id = at.ImageId
                            WHERE at.LibraryId = @LibraryId AND at.Id = @Id";
            await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleContentModel, AccountModel, AccountModel, TagModel, ArticleModel>(sql, (ar, a, c, con, writer, reviewer, t) =>
            {
                if (article == null)
                {
                    article = ar;
                    article.WriterAccountName = writer?.Name;
                    article.ReviewerAccountName = reviewer?.Name;
                }

                if (a != null && !article.Authors.Any(x => x.Id == a.Id))
                {
                    article.Authors.Add(a);
                }

                if (c != null && !article.Categories.Any(x => x.Id == c.Id))
                {
                    article.Categories.Add(c);
                }

                if (t != null && !article.Tags.Any(x => x.Id == t.Id))
                {
                    article.Tags.Add(t);
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
                            aw.Name As WriterAccountName,
                            arv.Name As ReviewerAccountName,
                            a.*, c.*, con.*, t.*
                        FROM Article at
                            LEFT OUTER JOIN ArticleAuthor ara ON ara.ArticleId = at.Id
                            LEFT OUTER JOIN Author a On ara.AuthorId = a.Id
                            LEFT OUTER JOIN ArticleFavorite af On at.Id = af.ArticleId 
                                AND (af.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleRead ar On at.Id = ar.ArticleId 
                                AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                            LEFT OUTER JOIN Category c ON ac.CategoryId = c.Id
                            LEFT OUTER JOIN ArticleTag att ON at.Id = att.ArticleId
                            LEFT OUTER JOIN Tag t ON att.TagId = t.Id
                            LEFT OUTER JOIN Accounts aw ON aw.Id = WriterAccountId
                            LEFT OUTER JOIN Accounts arv ON arv.Id = ReviewerAccountId
                            LEFT OUTER JOIN `File` fl ON fl.Id = at.ImageId
                            LEFT JOIN ArticleContent con ON con.ArticleId = at.Id
                        WHERE at.LibraryId = @LibraryId 
                            AND at.Id IN @ArticleList";
        var command3 = new CommandDefinition(sql3, new
        {
            LibraryId = libraryId,
            ArticleList = articleIds,
            AccountId = accountId
        }, cancellationToken: cancellationToken);

        await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleContentModel, TagModel, ArticleModel>(command3, (at, a, c, con, t) =>
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
            
            if (t != null && !article.Tags.Any(x => x.Id == t.Id))
            {
                article.Tags.Add(t);
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

    public async Task UpdateArticleImage(int libraryId, long articleId, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Article
                            SET ImageId = @ImageId
                            Where Id = @ArticleId AND LibraryId = @LibraryId;";
            var command = new CommandDefinition(sql, new { ImageId = imageId, ArticleId = articleId, LibraryId = libraryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task AddArticleToFavorites(int libraryId, int? accountId, long articleId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"REPLACE INTO ArticleFavorite (LibraryId, ArticleId, AccountId) VALUES (@LibraryId, @ArticleId, @AccountId)";
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

    public async Task<IEnumerable<ArticleModel>> GetAllArticles(int libraryId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT at.*, fl.FilePath AS ImageUrl,
                            CASE WHEN af.ArticleId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            a.*, c.*, con.*, t.*
                        FROM Article at
                            LEFT OUTER JOIN ArticleAuthor ara ON ara.ArticleId = at.Id
                            LEFT OUTER JOIN Author a On ara.AuthorId = a.Id
                            LEFT OUTER JOIN ArticleFavorite af On at.Id = af.ArticleId 
                                AND (af.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleRead ar On at.Id = ar.ArticleId 
                                AND (ar.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN ArticleCategory ac ON at.Id = ac.ArticleId
                            LEFT OUTER JOIN Category c ON ac.CategoryId = c.Id
                            LEFT OUTER JOIN ArticleTag att ON at.Id = att.ArticleId
                            LEFT OUTER JOIN Tag t ON att.TagId = t.Id
                            LEFT OUTER JOIN `File` fl ON fl.Id = at.ImageId
                            LEFT JOIN ArticleContent con ON con.ArticleId = at.Id
                        WHERE at.LibraryId = @LibraryId";

            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
            }, 
            cancellationToken: cancellationToken);

            var articles = new Dictionary<long, ArticleModel>();

            await connection.QueryAsync<ArticleModel, AuthorModel, CategoryModel, ArticleContentModel, TagModel, ArticleModel>(command, (at, a, c, con, t) =>
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
                
                if (t != null && !article.Tags.Any(x => x.Id == t.Id))
                {
                    article.Tags.Add(t);
                }

                if (con != null && !article.Contents.Any(x => x.Id == con.Id))
                {
                    article.Contents.Add(con);
                }

                return article;
            });

            return articles.Values.ToList();
        }
    }
}
