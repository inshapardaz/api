using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class BookShelfDataHelper
    {
        public static void AddBookShelf(this IDbConnection connection, BookShelfDto bookshelf)
        {
            var id = connection.ExecuteScalar<int>("INSERT INTO BookShelf (Name, [Description], ImageId, LibraryId, AccountId, IsPublic) " +
                "OUTPUT Inserted.Id " +
                "VALUES (@Name, @Description, @ImageId, @LibraryId, @AccountId, @IsPublic)", bookshelf);
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
            return connection.QuerySingleOrDefault<BookShelfDto>(@"Select s.* From BookShelf s
                                Inner Join BookShelfBook b ON s.Id = b.BookShelfId
                                Where b.Id = @BookId ", new { BookId = bookId });
        }

        public static int GetBookCountByBookShelf(this IDbConnection connection, int bookShelfId)
        {
            return connection.QuerySingleOrDefault<int>(@"Select Count(*) From BookShelf s
                                Inner Join BookShelfBook b ON s.Id = b.BookShelfId
                                Where s.Id = @BookShelfId", new { BookShelfId = bookShelfId });
        }

        public static IEnumerable<(int BookId, int Index)> GetBookShelfBooks(this IDbConnection connection, int bookShelfId)
        {
            return connection.Query<(int, int)>(@"SELECT BookId, [Index] FROM BookShelfBook
                                WHERE BookShelfId = @BookShelfId", new { BookShelfId = bookShelfId });
        }

        public static string GetBookShelfImageUrl(this IDbConnection connection, int bookShelfId)
        {
            return GetBookShelfImage(connection, bookShelfId)?.FilePath;
        }

        public static FileDto GetBookShelfImage(this IDbConnection connection, int bookShelfId)
        {
            var sql = @"Select f.* from [File] f
                        Inner Join BookShelf s ON f.Id = s.ImageId
                        Where s.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = bookShelfId });
        }
    }
}
