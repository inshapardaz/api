using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class CategoryRepository(MySqlConnectionProvider connectionProvider) : ICategoryRepository
{
    private const string SelectSql = @"SELECT c.Id, c.Name, c.ParentCategoryId, p.Name AS ParentCategoryName,
                                (SELECT Count(*) FROM BookCategory b WHERE b.CategoryId = c.Id) AS BookCount,
                                (SELECT Count(*) FROM PeriodicalCategory pc WHERE pc.CategoryId = c.Id) AS PeriodicalCount,
                                (SELECT Count(*) FROM ArticleCategory INNER JOIN Article on ArticleCategory.ArticleId = Article.Id WHERE ArticleCategory.CategoryId = c.Id AND Article.`Type` = 1) AS ArticleCount,
                                (SELECT Count(*) FROM ArticleCategory INNER JOIN Article on ArticleCategory.ArticleId = Article.Id WHERE ArticleCategory.CategoryId = c.Id AND Article.`Type` = 2) AS PoetryCount,
                                (SELECT Count(*) FROM Category ch WHERE ch.ParentCategoryId = c.Id) AS ChildCount
                            FROM Category AS c
                            LEFT JOIN Category AS p ON p.Id = c.ParentCategoryId";

    public async Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO Category(`Name`, LibraryId, ParentCategoryId)
                            VALUES (@Name, @LibraryId, @ParentCategoryId);
                            SELECT LAST_INSERT_ID()";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = category.Name, category.ParentCategoryId }, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetCategoryById(libraryId, id, cancellationToken);
    }

    public async Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Category
                            SET `Name` = @Name, ParentCategoryId = @ParentCategoryId
                            WHERE Id = @Id
                                AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { Id = category.Id, LibraryId = libraryId, Name = category.Name, category.ParentCategoryId }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM Category
                            WHERE LibraryId = @LibraryId
                                AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategories(int libraryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} WHERE c.LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }

    public async Task<CategoryModel> GetCategoryById(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} WHERE c.LibraryId = @LibraryId AND c.Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CategoryModel>(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategoriesByIds(int libraryId, IEnumerable<int> categoryIds, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} WHERE c.LibraryId = @LibraryId AND c.Id IN @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryIds }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetChildCategories(int libraryId, int parentCategoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} WHERE c.LibraryId = @LibraryId AND c.ParentCategoryId = @ParentCategoryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, ParentCategoryId = parentCategoryId }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }
}
