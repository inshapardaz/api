using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class BookPageDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddBookPage(this IDbConnection connection, BookPageDto bookPage)
        {
            var sql = _dbType == DatabaseTypes.SqlServer 
                ? @"INSERT INTO BookPage (BookId, ContentId, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                        Output Inserted.Id
                        VALUES (@BookId, @ContentId, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status)"
                : @"INSERT INTO BookPage (BookId, ContentId, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                        VALUES (@BookId, @ContentId, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status);
                    SELECT LAST_INSERT_ID();";
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
            var sql = "DELETE FROM BookPage WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = bookPages.Select(f => f.Id) });
        }

        public static BookPageDto GetBookPageByNumber(this IDbConnection connection, int bookId, int sequenceNumber)
        {
            var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber });

            return connection.QuerySingleOrDefault<BookPageDto>(command);
        }

        public static BookPageDto GetBookPageById(this IDbConnection connection, int bookId, long pageId)
        {
            var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND Id = @Id";
            var command = new CommandDefinition(sql, new { BookId = bookId, id = pageId });

            return connection.QuerySingleOrDefault<BookPageDto>(command);
        }

        public static int GetBookPageCount(this IDbConnection connection, int bookId)
        {
            var sql = @"SELECT Count(*)
                        FROM BookPage
                        WHERE BookId = @BookId";
            var command = new CommandDefinition(sql, new { BookId = bookId });

            return connection.QuerySingleOrDefault<int>(command);
        }
    }
}
