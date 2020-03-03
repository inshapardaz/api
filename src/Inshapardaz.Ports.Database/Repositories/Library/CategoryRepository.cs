using Dapper;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public CategoryRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Library.Category(Name, LibraryId) Output Inserted.Id Values(@Name, @LibraryId)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetCategoryById(libraryId, id, cancellationToken);
        }

        public async Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Category Set Name = @Name Where Id = @Id AND LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { Id = category.Id, LibraryId = libraryId, Name = category.Name }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.Category Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<CategoryModel>> GetCategories(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select c.Id, c.Name,
                            (Select Count(*) From Library.BookCategory b Where b.CategoryId = c.Id) AS BookCount
                            FROM Library.Category AS c
                            Where LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<CategoryModel>(command);
            }
        }

        public async Task<CategoryModel> GetCategoryById(int libraryId, int categoryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select c.Id, c.Name,
                            (Select Count(*) From Library.BookCategory b Where b.CategoryId = c.Id) AS BookCount
                            FROM Library.Category AS c
                            Where c.LibraryId = @LibraryId And c.Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = categoryId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<CategoryModel>(command);
            }
        }
    }
}
