using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;

public class AuthorRepository : IAuthorRepository
{
    private readonly SqlServerConnectionProvider _connectionProvider;

    public AuthorRepository(SqlServerConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<AuthorModel> AddAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken)
    {
        int authorId;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Insert Into Author(Name, Description, ImageId, LibraryId, AuthorType) OUTPUT Inserted.Id VALUES(@Name, @Description, @ImageId, @LibraryId, @AuthorType);";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = author.Name, Description = author.Description, ImageId = author.ImageId, AuthorType = (int)author.AuthorType }, cancellationToken: cancellationToken);
            authorId = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetAuthorById(libraryId, authorId, cancellationToken);
    }

    public async Task UpdateAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Author Set Name = @Name, Description = @Description, ImageId = @ImageId, AuthorType = @AuthorType Where Id = @Id AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { Id = author.Id, LibraryId = libraryId, Name = author.Name, Description = author.Description, ImageId = author.ImageId, AuthorType = (int)author.AuthorType }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task UpdateAuthorImage(int libraryId, int authorId, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Author Set ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId ";
            var command = new CommandDefinition(sql, new { Id = authorId, LibraryId = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task DeleteAuthor(int libraryId, int authorId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Author Where LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = authorId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<Page<AuthorModel>> GetAuthors(int libraryId, AuthorTypes? authorType, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT a.Id, a.Name, a.Description, a.AuthorType, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*) FROM BookAuthor WHERE AuthorId = a.Id) AS BookCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 2) AS PoetryCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            AND (@AuthorType IS NULL OR a.AuthorType = @AuthorType)
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    AuthorType = authorType,
                                                    PageSize = pageSize,
                                                    PageNumber = pageNumber
                                                },
                                                cancellationToken: cancellationToken);

            var authors = await connection.QueryAsync<AuthorModel>(command);

            var sqlAuthorCount = "SELECT COUNT(*) FROM Author WHERE LibraryId = @LibraryId AND (@AuthorType IS NULL OR AuthorType = @AuthorType)";
            var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, AuthorType = authorType }, cancellationToken: cancellationToken));

            return new Page<AuthorModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = authorCount,
                Data = authors
            };
        }
    }

    public async Task<AuthorModel> GetAuthorById(int libraryId, int authorId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT a.Id, a.Name, a.Description, a.AuthorType as AuthorType, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*) FROM BookAuthor WHERE AuthorId = a.Id) AS BookCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 2) AS PoetryCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            And a.Id = @AuthorId";
            var command = new CommandDefinition(sql,
                                                new { LibraryId = libraryId, AuthorId = authorId },
                                                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<AuthorModel>(command);
        }
    }

    public async Task<Page<AuthorModel>> FindAuthors(int libraryId, string query, AuthorTypes? authorType, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT a.Id, a.Name, a.Description, a.AuthorType, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*) FROM BookAuthor WHERE AuthorId = a.Id) AS BookCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 2) AS PoetryCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            AND a.Name LIKE @Query
                            AND (@AuthorType IS NULL OR a.AuthorType = @AuthorType)
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            var command = new CommandDefinition(sql,
                                                new { LibraryId = libraryId, Query = $"%{query}%", AuthorType = authorType, PageSize = pageSize, PageNumber = pageNumber },
                                                cancellationToken: cancellationToken);

            var authors = await connection.QueryAsync<AuthorModel>(command);

            var sqlAuthorCount = "SELECT COUNT(*) FROM Author WHERE LibraryId = @LibraryId And Name LIKE @Query AND (@AuthorType IS NULL OR AuthorType = @AuthorType)";
            var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, Query = $"%{query}%", AuthorType = authorType }, cancellationToken: cancellationToken));

            return new Page<AuthorModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = authorCount,
                Data = authors
            };
        }
    }

    public async Task<IEnumerable<AuthorModel>> GetAuthorByIds(int libraryId, IEnumerable<int> authorIds, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT a.Id, a.Name, a.Description, a.AuthorType, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*) FROM BookAuthor WHERE AuthorId = a.Id) AS BookCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM ArticleAuthor INNER JOIN Article on ArticleAuthor.ArticleId = Article.Id WHERE ArticleAuthor.AuthorId = a.Id AND Article.`Type` = 2) AS PoetryCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            And a.Id IN @AuthorIds";
            var command = new CommandDefinition(sql,
                                                new { LibraryId = libraryId, AuthorIds = authorIds },
                                                cancellationToken: cancellationToken);

            return await connection.QueryAsync<AuthorModel>(command);
        }
    }
}
