using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class CategoryDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddCategory(this IDbConnection connection, CategoryDto category)
        {
            var id = _dbType == DatabaseTypes.SqlServer
                ? connection.ExecuteScalar<int>("INSERT INTO Category (Name, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @LibraryId)", category)
                : connection.ExecuteScalar<int>("INSERT INTO Category (`Name`, LibraryId) VALUES (@Name, @LibraryId); SELECT LAST_INSERT_ID();", category);
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
            var sql = "DELETE FROM Category WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = categories.Select(a => a.Id) });
        }

        public static CategoryDto GetCategoryById(this IDbConnection connection, int libraryId, int id)
        {
            return connection.QuerySingleOrDefault<CategoryDto>("SELECT * FROM Category WHERE Id = @Id AND LibraryId = @LibraryId",
                new { Id = id, LibraryId = libraryId });
        }

        public static bool DoesCategoryExists(this IDbConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<CategoryDto> GetCategoriesByBook(this IDbConnection connection, int id)
        {
            return connection.Query<CategoryDto>(@"SELECT c.* FROM Category c
                                INNER JOIN BookCategory bc ON c.Id = bc.CategoryId
                                WHERE bc.BookId = @BookId ", new { BookId = id });
        }

        public static IEnumerable<CategoryDto> GetCategoriesByPeriodical(this IDbConnection connection, int id)
        {
            return connection.Query<CategoryDto>(@"SELECT c.* From Category c
                                INNER JOIN PeriodicalCategory pc ON c.Id = pc.CategoryId
                                WHERE pc.PeriodicalId = @PeriodicalId", new { PeriodicalId = id });
        }

        public static IEnumerable<CategoryDto> GetCategoriesByArticle(this IDbConnection connection, long id)
        {
            return connection.Query<CategoryDto>(@"SELECT c.* FROM Category c
                                INNER JOIN ArticleCategory bc ON c.Id = bc.CategoryId
                                WHERE bc.ArticleId = @ArticleId ", new { ArticleId = id });
        }

        public static void AddBooksToCategory(this IDbConnection connection, IEnumerable<BookDto> books, CategoryDto category)
        {
            foreach (var book in books)
            {
                connection.Execute("INSERT INTO BookCategory (BookId, CategoryId) VALUES(@BookId, @CategoryId)",
                    new { BookId = book.Id, CategoryId = category.Id });
            }
        }

        public static void AddBookToCategories(this IDbConnection connection, int bookId, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO BookCategory (BookId, CategoryId) VALUES(@BookId, @CategoryId)",
                    new { BookId = bookId, CategoryId = category.Id });
            }
        }

        public static void AddPeriodicalToCategory(this IDbConnection connection, IEnumerable<PeriodicalDto> periodicals, CategoryDto category)
        {
            foreach (var periodical in periodicals)
            {
                connection.Execute("INSERT INTO PeriodicalCategory (PeriodicalId, CategoryId) VALUES(@PeriodicalId, @CategoryId)",
                    new { PeriodicalId = periodical.Id, CategoryId = category.Id });
            }
        }

        public static void AddPeriodicalToCategories(this IDbConnection connection, int periodicalId, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO PeriodicalCategory (PeriodicalId, CategoryId) VALUES(@PeriodicalId, @CategoryId)",
                    new { PeriodicalId = periodicalId, CategoryId = category.Id });
            }
        }

        public static void AddArticleToCategories(this IDbConnection connection, long articleId, IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO ArticleCategory (ArticleId, CategoryId) VALUES(@ArticleId, @CategoryId)",
                    new { ArticleId = articleId, CategoryId = category.Id });
            }
        }
    }
}
