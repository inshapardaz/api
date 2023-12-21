using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    //TODO: Add inner join to book to verify library
    public class BookPageRepository : IBookPageRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public BookPageRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<BookPageModel> AddPage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, int? chapterId, CancellationToken cancellationToken)
        {
            int pageId;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into BookPage(BookId, SequenceNumber, Text, ImageId, ChapterId)
                            OUTPUT Inserted.Id
                            VALUES(@BookId, @SequenceNumber, @Text, @ImageId, @ChapterId);";
                var command = new CommandDefinition(sql, new { 
                    BookId = bookId, 
                    SequenceNumber = sequenceNumber, 
                    Text = text, 
                    ImageId = imageId, 
                    ChapterId = chapterId 
                }, cancellationToken: cancellationToken);
                pageId = await connection.ExecuteScalarAsync<int>(command);
            }

            await ReorderPages(libraryId, bookId, cancellationToken);
            return await GetPageById(pageId, cancellationToken);
        }

        public async Task<int> AddPage(int libraryId, BookPageModel page, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into BookPage(
                                Text,
                                SequenceNumber,
                                BookId,
                                ImageId,
                                Status,
                                WriterAccountId,
                                WriterAssignTimeStamp,
                                ReviewerAccountId,
                                ReviewerAssignTimeStamp,
                                ChapterId
                             )
                            OUTPUT Inserted.Id
                            VALUES(
                                @Text,
                                @SequenceNumber,
                                @BookId,
                                @ImageId,
                                @Status,
                                @WriterAccountId,
                                @WriterAssignTimeStamp,
                                @ReviewerAccountId,
                                @ReviewerAssignTimeStamp,
                                @ChapterId)";
                var command = new CommandDefinition(sql, page, cancellationToken: cancellationToken);
                return await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task<BookPageModel> GetPageBySequenceNumber(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Text, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            Where b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, BookId = bookId, SequenceNumber = sequenceNumber },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<BookPageModel>(command);
            }
        }

        public async Task DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Delete p From BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                await ReorderPages(libraryId, bookId, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdatePage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, EditingStatus status, int? chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.Text = @Text, 
                                p.ImageId = @ImageId, 
                                Status = @Status,
                                ChapterId = @ChapterId
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    Text = text,
                    ImageId = imageId,
                    BookId = bookId,
                    SequenceNumber = sequenceNumber,
                    Status = status,
                    ChapterId = chapterId
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.ImageId = @ImageId
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, ImageId = imageId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task DeletePageImage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET ImageId = NULL
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
        public async Task<int> GetPageCount(int libraryId, int bookId, int oldSequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())

            {
                var sql = @"SELECT COUNT(p.*)
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
                return await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task<Page<BookPageModel>> GetPagesByBook(int libraryId, int bookId, int pageNumber, int pageSize, EditingStatus status, AssignmentFilter assignmentFilter, AssignmentFilter reviewerAssignmentFilter, int? assignedTo, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, 
                                   p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp,
                                   p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp,
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId
                            AND (@Status = -1 OR p.Status = @Status )
                            AND (
                                ( @AssignmentFilter = 0 ) OR
                                ( @AssignmentFilter = 1 AND p.WriterAccountId IS NOT NULL) OR
                                ( @AssignmentFilter = 2 AND p.WriterAccountId IS NULL) OR
                                ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.WriterAccountId = @AccountId )
                            )
                            AND (
                                ( @ReviewerAssignmentFilter = 0 ) OR
                                ( @ReviewerAssignmentFilter = 1 AND p.ReviewerAccountId IS NOT NULL) OR
                                ( @ReviewerAssignmentFilter = 2 AND p.ReviewerAccountId IS NULL) OR
                                ( (@ReviewerAssignmentFilter = 3  OR @ReviewerAssignmentFilter = 4) AND p.ReviewerAccountId = @AccountId )
                            )
                            ORDER BY p.SequenceNumber
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        BookId = bookId,
                                                        Status = status,
                                                        PageSize = pageSize,
                                                        PageNumber = pageNumber,
                                                        AssignmentFilter = assignmentFilter,
                                                        ReviewerAssignmentFilter = reviewerAssignmentFilter,
                                                        AccountId = assignedTo
                                                    },
                                                    cancellationToken: cancellationToken);

                var pages = await connection.QueryAsync<BookPageModel>(command);

                var sqlCount = @"SELECT Count(*)
                                FROM BookPage p INNER JOIN Book b ON b.Id = p.BookId
                                WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId
                                AND (@Status = -1 OR p.Status = @Status )
                                AND (
                                    ( @AssignmentFilter = 0 ) OR
                                    ( @AssignmentFilter = 1 AND p.WriterAccountId IS NOT NULL) OR
                                    ( @AssignmentFilter = 2 AND p.WriterAccountId IS NULL) OR
                                    ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.WriterAccountId = @AccountId )
                                )
                                AND (
                                ( @ReviewerAssignmentFilter = 0 ) OR
                                ( @ReviewerAssignmentFilter = 1 AND p.ReviewerAccountId IS NOT NULL) OR
                                ( @ReviewerAssignmentFilter = 2 AND p.ReviewerAccountId IS NULL) OR
                                ( (@ReviewerAssignmentFilter = 3  OR @ReviewerAssignmentFilter = 4) AND p.ReviewerAccountId = @AccountId )
                            )";
                var commandCount = new CommandDefinition(sqlCount, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    Status = status,
                    AssignmentFilter = assignmentFilter,
                    ReviewerAssignmentFilter = reviewerAssignmentFilter,
                    AccountId = assignedTo
                },
                    cancellationToken: cancellationToken);

                var pagesCount = await connection.QuerySingleAsync<int>(commandCount);
                return new Page<BookPageModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = pagesCount,
                    Data = pages
                };
            }
        }

        public async Task<BookPageModel> UpdateWriterAssignment(int libraryId, int bookId, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.WriterAccountId = @WriterAccountId, p.WriterAssignTimeStamp = GETUTCDATE()
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            Where b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, WriterAccountId = assignedAccountId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdateReviewerAssignment(int libraryId, int bookId, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.ReviewerAccountId = @ReviewerAccountId, p.ReviewerAssignTimeStamp = GETUTCDATE()
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            Where b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, ReviewerAccountId = assignedAccountId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<int> GetLastPageNumberForBook(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT Max(p.SequenceNumber)
                            FROM BookPage AS p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, BookId = bookId },
                                                    cancellationToken: cancellationToken);

                return await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task<IEnumerable<BookPageModel>> GetAllPagesByBook(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, 
                            p.WriterAccountId, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId
                            ORDER BY p.SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<BookPageModel>(command);
            }
        }

        public async Task<Page<BookPageModel>> GetPagesByUser(int libraryId, int accountId, EditingStatus statusFilter, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, 
                                   p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp,
                                   p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp,
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE b.LibraryId = @LibraryId 
                            AND p.ReviewerAccountId = @AccountId OR p.WriterAccountId = @AccountId
                            AND (@Status = -1 OR p.Status = @Status )
                            ORDER BY p.BookId, p.SequenceNumber
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        Status = statusFilter,
                                                        PageSize = pageSize,
                                                        PageNumber = pageNumber,
                                                        AccountId = accountId
                                                    },
                                                    cancellationToken: cancellationToken);

                var pages = await connection.QueryAsync<BookPageModel>(command);

                var sqlCount = @"SELECT Count(*)
                                FROM BookPage p INNER JOIN Book b ON b.Id = p.BookId
                                WHERE b.LibraryId = @LibraryId 
                                AND p.ReviewerAccountId = @AccountId OR p.WriterAccountId = @AccountId
                                AND (@Status = -1 OR p.Status = @Status )";
                var commandCount = new CommandDefinition(sqlCount, new
                {
                    LibraryId = libraryId,
                    Status = statusFilter,
                    AccountId = accountId
                },
                    cancellationToken: cancellationToken);

                var pagesCount = await connection.QuerySingleAsync<int>(commandCount);
                return new Page<BookPageModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = pagesCount,
                    Data = pages
                };
            }
        }

        public async Task<IEnumerable<BookPageModel>> GetPagesByBookChapter(int libraryId, int bookId, int chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, 
                            p.WriterAccountId, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId And c.Id = @ChapterId
                            ORDER BY p.SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, ChapterId = chapterId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<BookPageModel>(command);
            }
        }

        public async Task ReorderPages(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.Id, row_number() OVER (ORDER BY p.SequenceNumber) as 'SequenceNumber'
                            From BookPage p
                            Inner Join Book b On b.Id = p.BookId
                            Where p.BookId = @BookId And b.LibraryId = @LibraryId
                            Order By p.SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId
                }, cancellationToken: cancellationToken);
                var newOrder = await connection.QueryAsync(command);

                var sql2 = @"UPDATE BookPage
                            SET SequenceNumber = @SequenceNumber
                            Where Id = @Id";
                var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task UpdatePageSequenceNumber(int libraryId, int bookId, int oldSequenceNumber, int newSequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {

                var sql = @"DECLARE @maxPosition INT;
                            DECLARE @Id INT;

                            SELECT @Id = Id
                            FROM BookPage
                            WHERE BookId = @BookId AND SequenceNumber = @oldPosition;

                            SELECT @maxPosition = MAX(SequenceNumber) 
                            FROM BookPage 
                            WHERE BookId = @BookId

                            IF (@newPosition < 1)
                             SET @newPosition = 1
 
                            IF (@newPosition > @maxPosition)
                             SET @newPosition = @maxPosition

                            UPDATE BookPage SET SequenceNumber = CASE
                                WHEN Id = @Id THEN @newPosition
                                WHEN @oldPosition < @newPosition THEN SequenceNumber - 1
                                WHEN @oldPosition > @newPosition THEN SequenceNumber + 1
                            END
                            WHERE BookId = @BookId AND SequenceNumber BETWEEN
                                CASE WHEN @oldPosition < @newPosition THEN @oldPosition ELSE @newPosition END AND
                                CASE WHEN @oldPosition > @newPosition THEN @oldPosition ELSE @newPosition END;
";
                var command = new CommandDefinition(sql, new
                {
                    BookId = bookId,
                    oldPosition = oldSequenceNumber,
                    newPosition = newSequenceNumber,

                }, cancellationToken: cancellationToken);
                
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<UserPageSummaryItem>> GetUserPageSummary(int libraryId, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT 1 As Status, Count(BookPage.Id) As Count 
                            FROM BookPage INNER JOIN Book ON Book.Id = BookPage.BookId
                            WHERE Book.LibraryId = @LibraryId AND BookPage.[Status] = 1 AND BookPage.WriterAccountId = @AccountId
                            UNION
                            SELECT 2 As Status, Count(BookPage.Id) As Count 
                            FROM BookPage INNER JOIN Book ON Book.Id = BookPage.BookId
                            WHERE Book.LibraryId = @LibraryId AND BookPage.[Status] = 2 AND BookPage.WriterAccountId = @AccountId
                            UNION
                            SELECT 3 As Status, Count(BookPage.Id) As Count 
                            FROM BookPage INNER JOIN Book ON Book.Id = BookPage.BookId
                            WHERE Book.LibraryId = @LibraryId AND BookPage.[Status] = 3 AND BookPage.ReviewerAccountId = @AccountId
                            UNION
                            SELECT 4 As Status, Count(BookPage.Id) As Count 
                            FROM BookPage INNER JOIN Book ON Book.Id = BookPage.BookId
                            WHERE Book.LibraryId = @LibraryId AND BookPage.[Status] = 4 AND BookPage.ReviewerAccountId = @AccountId";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    AccountId = accountId
                }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<UserPageSummaryItem>(command);
            }
        }

        private async Task<BookPageModel> GetPageById(int pageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Text, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p 
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            Where p.id= @PageId";
                var command = new CommandDefinition(sql, new { PageId = pageId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<BookPageModel>(command);
            }
        }

    }
}
