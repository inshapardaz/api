using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;

public class CategoryRepository(SqlServerConnectionProvider connectionProvider) : ICategoryRepository
{
    private const string SelectSql = @"Select c.Id, c.Name, c.ParentCategoryId, p.Name AS ParentCategoryName,
                            (Select Count(*) From BookCategory b Where b.CategoryId = c.Id) AS BookCount,
                            (SELECT Count(*) FROM PeriodicalCategory pc WHERE pc.CategoryId = c.Id) AS PeriodicalCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN Article on articlecategory.ArticleId = Article.Id WHERE articlecategory.CategoryId = c.Id AND Article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN Article on articlecategory.ArticleId = Article.Id WHERE articlecategory.CategoryId = c.Id AND Article.`Type` = 2) AS PoetryCount,
                            (SELECT Count(*) FROM Category ch WHERE ch.ParentCategoryId = c.Id) AS ChildCount
                            FROM Category AS c
                            LEFT JOIN Category AS p ON p.Id = c.ParentCategoryId";

    public async Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = "Insert Into Category(Name, LibraryId, ParentCategoryId) Output Inserted.Id Values(@Name, @LibraryId, @ParentCategoryId)";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = category.Name, category.ParentCategoryId }, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetCategoryById(libraryId, id, cancellationToken);
    }

    public async Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Category Set Name = @Name, ParentCategoryId = @ParentCategoryId Where Id = @Id AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { Id = category.Id, LibraryId = libraryId, Name = category.Name, category.ParentCategoryId }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Category Where LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategories(int libraryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} Where c.LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }

    public async Task<CategoryModel> GetCategoryById(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} Where c.LibraryId = @LibraryId And c.Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CategoryModel>(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategoriesByIds(int libraryId, IEnumerable<int> categoryIds, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} Where c.LibraryId = @LibraryId And c.Id IN @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryIds }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetChildCategories(int libraryId, int parentCategoryId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = $"{SelectSql} Where c.LibraryId = @LibraryId And c.ParentCategoryId = @ParentCategoryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, ParentCategoryId = parentCategoryId }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }
}
