using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class CategoryDataHelper
    {
        public static void AddCategory(this IDbConnection connection, CategoryDto category)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Library.Category (Name, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @LibraryId)", category);
            category.Id = id;
        }

        public static void AddCategories(this IDbConnection connection, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                AddCategory(connection, category);
            }
        }

        public static void DeleteCategries(this IDbConnection connection, IEnumerable<CategoryDto> categories)
        {
            var sql = "Delete From Library.Category Where Id IN @Ids";
            connection.Execute(sql, new { Ids = categories.Select(a => a.Id) });
        }

        public static CategoryDto GetCategoryById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<CategoryDto>("Select * From Library.Category Where Id = @Id", new { Id = id });
        }

        public static IEnumerable<CategoryDto> GetCategoriesByBook(this IDbConnection connection, int id)
        {
            return connection.Query<CategoryDto>(@"Select c.* From Library.Category c
                                Inner Join Library.BookCategory bc ON c.Id = bc.CategoryId
                                Where bc.BookId = @BookId ", new { BookId = id });
        }

        public static void AddBooksToCategory(this IDbConnection connection, IEnumerable<BookDto> books, CategoryDto category)
        {
            foreach (var book in books)
            {
                connection.Execute("Insert Into Library.BookCategory (BookId, CategoryId) Values(@BookId, @CategoryId)",
                    new { BookId = book.Id, CategoryId = category.Id });
            }
        }
    }
}
