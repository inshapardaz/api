using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class CategoryDataHelper
    {
        public static void AddCategory(this IDbConnection connection, CategoryDto category)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Category (Name, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @LibraryId)", category);
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
            var sql = "Delete From Category Where Id IN @Ids";
            connection.Execute(sql, new { Ids = categories.Select(a => a.Id) });
        }

        public static CategoryDto GetCategoryById(this IDbConnection connection, int libraryId, int id)
        {
            return connection.QuerySingleOrDefault<CategoryDto>("Select * From Category Where Id = @Id AND LibraryId = @LibraryId",
                new { Id = id, LibraryId = libraryId });
        }

        public static bool DoesCategoryExists(this IDbConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<CategoryDto> GetCategoriesByBook(this IDbConnection connection, int id)
        {
            return connection.Query<CategoryDto>(@"Select c.* From Category c
                                Inner Join BookCategory bc ON c.Id = bc.CategoryId
                                Where bc.BookId = @BookId ", new { BookId = id });
        }

        public static IEnumerable<CategoryDto> GetCategoriesByPeriodical(this IDbConnection connection, int id)
        {
            return connection.Query<CategoryDto>(@"Select c.* From Category c
                                Inner Join PeriodicalCategory pc ON c.Id = pc.CategoryId
                                Where pc.PeriodicalId = @PeriodicalId", new { PeriodicalId = id });
        }

        public static void AddBooksToCategory(this IDbConnection connection, IEnumerable<BookDto> books, CategoryDto category)
        {
            foreach (var book in books)
            {
                connection.Execute("Insert Into BookCategory (BookId, CategoryId) Values(@BookId, @CategoryId)",
                    new { BookId = book.Id, CategoryId = category.Id });
            }
        }

        public static void AddBookToCategories(this IDbConnection connection, int bookId, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("Insert Into BookCategory (BookId, CategoryId) Values(@BookId, @CategoryId)",
                    new { BookId = bookId, CategoryId = category.Id });
            }
        }

        public static void AddPeriodicalToCategory(this IDbConnection connection, IEnumerable<PeriodicalDto> periodicals, CategoryDto category)
        {
            foreach (var periodical in periodicals)
            {
                connection.Execute("Insert Into PeriodicalCategory (PeriodicalId, CategoryId) Values(@PeriodicalId, @CategoryId)",
                    new { PeriodicalId = periodical.Id, CategoryId = category.Id });
            }
        }

        public static void AddPeriodicalToCategories(this IDbConnection connection, int periodicalId, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("Insert Into PeriodicalCategory (PeriodicalId, CategoryId) Values(@PeriodicalId, @CategoryId)",
                    new { PeriodicalId = periodicalId, CategoryId = category.Id });
            }
        }
    }
}
