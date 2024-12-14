using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;

public class CategoryRepository : ICategoryRepository
{
    private readonly SqlServerConnectionProvider _connectionProvider;

    public CategoryRepository(SqlServerConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = "Insert Into Category(Name, LibraryId) Output Inserted.Id Values(@Name, @LibraryId)";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetCategoryById(libraryId, id, cancellationToken);
    }

    public async Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Category Set Name = @Name Where Id = @Id AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { Id = category.Id, LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Category Where LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategories(int libraryId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select c.Id, c.Name,
                            (Select Count(*) From BookCategory b Where b.CategoryId = c.Id) AS BookCount,
                            (SELECT Count(*) FROM PeriodicalCategory pc WHERE pc.CategoryId = c.Id) AS PeriodicalCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 2) AS PoetryCount
                            FROM Category AS c
                            Where LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }

    public async Task<CategoryModel> GetCategoryById(int libraryId, int categoryId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select c.Id, c.Name,
                            (Select Count(*) From BookCategory b Where b.CategoryId = c.Id) AS BookCount,
                            (SELECT Count(*) FROM PeriodicalCategory pc WHERE pc.CategoryId = c.Id) AS PeriodicalCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 2) AS PoetryCount
                            FROM Category AS c
                            Where c.LibraryId = @LibraryId And c.Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CategoryModel>(command);
        }
    }

    public async Task<IEnumerable<CategoryModel>> GetCategoriesByIds(int libraryId, IEnumerable<int> categoryIds, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select c.Id, c.Name,
                            (Select Count(*) From BookCategory b Where b.CategoryId = c.Id) AS BookCount,
                            (SELECT Count(*) FROM PeriodicalCategory pc WHERE pc.CategoryId = c.Id) AS PeriodicalCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 1) AS ArticleCount,
                            (SELECT Count(*) FROM articlecategory INNER JOIN article on articlecategory.ArticleId = article.Id WHERE articlecategory.CategoryId = c.Id AND article.`Type` = 2) AS PoetryCount
                            FROM Category AS c
                            Where c.LibraryId = @LibraryId And c.Id IN @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryIds }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CategoryModel>(command);
        }
    }
}
