using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class BookShelfDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddBookShelf(this IDbConnection connection, BookShelfDto bookshelf)
        {
            var sql = _dbType == DatabaseTypes.SqlServer 
                ? @"INSERT INTO BookShelf (Name, [Description], ImageId, LibraryId, AccountId, IsPublic)
                    OUTPUT Inserted.Id
                    VALUES (@Name, @Description, @ImageId, @LibraryId, @AccountId, @IsPublic)"
                : @"INSERT INTO BookShelf (Name, `Description`, ImageId, LibraryId, AccountId, IsPublic)
                    VALUES (@Name, @Description, @ImageId, @LibraryId, @AccountId, @IsPublic);
                    SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(sql, bookshelf);
            bookshelf.Id = id;
        }

        public static void AddBookShelves(this IDbConnection connection, IEnumerable<BookShelfDto> bookshelves)
        {
            foreach (var bookShelf in bookshelves)
            {
                AddBookShelf(connection, bookShelf);
            }
        }

        public static void AddBookstToBookShelf(this IDbConnection connection, int bookshelfId, IEnumerable<int> booksIds)
        {
            var id = connection.Execute("INSERT INTO BookShelfBook (BookShelfId, BookId) " +
                "VALUES (@BookShelfId, @BookId)", booksIds.Select(bId => new { BookShelfId = bookshelfId, BookId = bId }));
        }

        public static void DeleteBookShelf(this IDbConnection connection, IEnumerable<BookShelfDto> bookshelves)
        {
            var sql = "Delete From BookShelf Where Id IN @Ids";
            connection.Execute(sql, new { Ids = bookshelves.Select(a => a.Id) });
        }

        public static BookShelfDto GetBookShelfById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<BookShelfDto>("Select * From BookShelf Where Id = @Id", new { Id = id });
        }

        public static BookShelfDto GetBookShelfForBook(this IDbConnection connection, int bookId)
        {
            return connection.QuerySingleOrDefault<BookShelfDto>(@"SELECT s.* FROM BookShelf s
                                INNER JOIN BookShelfBook b ON s.Id = b.BookShelfId
                                WHERE b.Id = @BookId ", new { BookId = bookId });
        }

        public static int GetBookCountByBookShelf(this IDbConnection connection, int bookShelfId)
        {
            return connection.QuerySingleOrDefault<int>(@"SELECT Count(*) FROM BookShelf s
                                INNER JOIN BookShelfBook b ON s.Id = b.BookShelfId
                                WHERE s.Id = @BookShelfId", new { BookShelfId = bookShelfId });
        }

        public static IEnumerable<(int BookId, int Index)> GetBookShelfBooks(this IDbConnection connection, int bookShelfId)
        {
            var Sql = _dbType == DatabaseTypes.SqlServer 
                    ? @"SELECT BookId, [Index] FROM BookShelfBook WHERE BookShelfId = @BookShelfId"
                    : @"SELECT BookId, `Index` FROM BookShelfBook WHERE BookShelfId = @BookShelfId";
            return connection.Query<(int, int)>(Sql, new { BookShelfId = bookShelfId });
        }

        public static string GetBookShelfImageUrl(this IDbConnection connection, int bookShelfId)
        {
            return GetBookShelfImage(connection, bookShelfId)?.FilePath;
        }

        public static FileDto GetBookShelfImage(this IDbConnection connection, int bookShelfId)
        {
            var sql = _dbType == DatabaseTypes.SqlServer
                    ? @"SELECT f.* FROM [File] f
                        INNER JOIN BookShelf s ON f.Id = s.ImageId
                        WHERE s.Id = @Id"
                    : @"SELECT f.* FROM `File` f
                        INNER JOIN BookShelf s ON f.Id = s.ImageId
                        WHERE s.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = bookShelfId });
        }
    }
}
