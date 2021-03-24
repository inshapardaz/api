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
            var sql = @"Insert Into BookPage (BookId, Text, SequenceNumber, ImageId, AccountId, Status)
                        Output Inserted.Id
                        Values (@BookId, @Text, @SequenceNumber, @ImageId, @AccountId, @Status)";
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

        public static BookPageDto GetBookPageByNumber(this IDbConnection connection, int bookId, int sequenceNumber)
        {
            var sql = @"SELECT *
                        FROM BookPage
                        Where BookId = @BookId AND SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber });

            return connection.QuerySingleOrDefault<BookPageDto>(command);
        }
    }
}
