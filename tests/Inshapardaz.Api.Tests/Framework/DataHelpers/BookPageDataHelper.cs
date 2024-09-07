using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IBookPageTestRepository
    {
        void AddBookPage(BookPageDto bookPage);

        void AddBookPages(IEnumerable<BookPageDto> bookPages);

        void DeleteBookPages(IEnumerable<BookPageDto> bookPages);

        IEnumerable<BookPageDto> GetBookPages(int bookId);
        BookPageDto GetBookPageByNumber(int bookId, int sequenceNumber);

        BookPageDto GetBookPageById(int bookId, long pageId);

        int GetBookPageCount(int bookId);
    }

    public class MySqlBookPageTestRepository : IBookPageTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlBookPageTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddBookPage(BookPageDto bookPage)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO BookPage (BookId, ContentId, ChapterId, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                        VALUES (@BookId, @ContentId, @ChapterId, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, bookPage);
                bookPage.Id = id;
            }
        }

        public void AddBookPages(IEnumerable<BookPageDto> bookPages)
        {
            foreach (var bookPage in bookPages)
            {
                AddBookPage(bookPage);
            }
        }

        public void DeleteBookPages(IEnumerable<BookPageDto> bookPages)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM BookPage WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = bookPages.Select(f => f.Id) });
            }
        }

        public IEnumerable<BookPageDto> GetBookPages(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId });

                return connection.Query<BookPageDto>(command);
            }
        }
        
        public BookPageDto GetBookPageByNumber(int bookId, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber });

                return connection.QuerySingleOrDefault<BookPageDto>(command);
            }
        }

        public BookPageDto GetBookPageById(int bookId, long pageId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND Id = @Id";
                var command = new CommandDefinition(sql, new { BookId = bookId, id = pageId });

                return connection.QuerySingleOrDefault<BookPageDto>(command);
            }
        }

        public int GetBookPageCount(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT Count(*)
                        FROM BookPage
                        WHERE BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId });

                return connection.QuerySingleOrDefault<int>(command);
            }
        }
    }

    public class SqlServerBookPageTestRepository : IBookPageTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerBookPageTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddBookPage(BookPageDto bookPage)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO BookPage (BookId, ContentId, SequenceNumber, ImageId, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, Status)
                        Output Inserted.Id
                        VALUES (@BookId, @ContentId, @SequenceNumber, @ImageId, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @Status)";
                var id = connection.ExecuteScalar<int>(sql, bookPage);
                bookPage.Id = id;
            }
        }

        public void AddBookPages(IEnumerable<BookPageDto> bookPages)
        {
            foreach (var bookPage in bookPages)
            {
                AddBookPage(bookPage);
            }
        }

        public void DeleteBookPages(IEnumerable<BookPageDto> bookPages)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM BookPage WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = bookPages.Select(f => f.Id) });
            }
        }

        public BookPageDto GetBookPageByNumber(int bookId, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber });

                return connection.QuerySingleOrDefault<BookPageDto>(command);
            }
        }
        
        public IEnumerable<BookPageDto> GetBookPages(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId });

                return connection.Query<BookPageDto>(command);
            }
        }

        public BookPageDto GetBookPageById(int bookId, long pageId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                        FROM BookPage
                        WHERE BookId = @BookId AND Id = @Id";
                var command = new CommandDefinition(sql, new { BookId = bookId, id = pageId });

                return connection.QuerySingleOrDefault<BookPageDto>(command);
            }
        }

        public int GetBookPageCount(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT Count(*)
                        FROM BookPage
                        WHERE BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId });

                return connection.QuerySingleOrDefault<int>(command);
            }
        }
    }
}
