using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public CategoryRepository(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO Category(`Name`, LibraryId)
                            VALUES (@Name, @LibraryId);
                            SELECT LAST_INSERT_ID()";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetCategoryById(libraryId, id, cancellationToken);
        }

        public async Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Category
                            SET `Name` = @Name 
                            WHERE Id = @Id 
                                AND LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { Id = category.Id, LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
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
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT c.Id, c.Name,
                                (SELECT Count(*) FROM BookCategory b WHERE b.CategoryId = c.Id) AS BookCount
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
                var sql = @"SELECT c.Id, c.Name,
                                (SELECT Count(*) FROM BookCategory b WHERE b.CategoryId = c.Id) AS BookCount
                            FROM Category AS c
                            WHERE c.LibraryId = @LibraryId
                                AND c.Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<CategoryModel>(command);
            }
        }

        public async Task<IEnumerable<CategoryModel>> GetCategoriesByIds(int libraryId, IEnumerable<int> categoryIds, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT c.Id, c.Name,
                                (SELECT COUNT(*) FROM BookCategory b WHERE b.CategoryId = c.Id) AS BookCount
                            FROM Category AS c
                            WHERE c.LibraryId = @LibraryId 
                                AND c.Id IN @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryIds }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<CategoryModel>(command);
            }
        }
    }
}
