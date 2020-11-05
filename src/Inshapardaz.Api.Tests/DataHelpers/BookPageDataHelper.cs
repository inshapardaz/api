using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class BookPageDataHelper
    {
        public static void AddBookPage(this IDbConnection connection, BookPageDto bookPage)
        {
            var sql = @"Insert Into BookPage (BookId, Text, PageNumber, ImageId)
                        Output Inserted.Id
                        Values (@BookId, @Text, @PageNumber, @ImageId)";
            var id = connection.ExecuteScalar<int>(sql, bookPage);
            bookPage.Id = id;
        }

        public static void AddBookPages(this IDbConnection connection, IEnumerable<BookPageDto> bookPages)
        {
            foreach (var bookPage in bookPages)
            {
                AddBookPage(connection, bookPage);
            }
        }

        public static void DeleteBookPages(this IDbConnection connection, IEnumerable<BookPageDto> bookPages)
        {
            var sql = "Delete From BookPage Where Id IN @Ids";
            connection.Execute(sql, new { Ids = bookPages.Select(f => f.Id) });
        }

        public static BookPageDto GetBookPageByNumber(this IDbConnection connection, int bookId, int pageNumber)
        {
            var sql = @"SELECT *
                        FROM BookPage
                        Where BookId = @BookId AND PageNumber = @PageNumber";
            var command = new CommandDefinition(sql, new { BookId = bookId, PageNumber = pageNumber });

            return connection.QuerySingle<BookPageDto>(command);
        }
    }
}
