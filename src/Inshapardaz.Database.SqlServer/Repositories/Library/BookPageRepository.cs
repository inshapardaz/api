﻿using Dapper;
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
            int authorId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into BookPage(BookId, SequenceNumber, Text, ImageId, ChapterId)
                            OUTPUT Inserted.Id
                            VALUES(@BookId, @SequenceNumber, @Text, @ImageId, @ChapterId);";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber, Text = text, ImageId = imageId, ChapterId = chapterId }, cancellationToken: cancellationToken);
                authorId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
        }

        public async Task<BookPageModel> GetPageBySequenceNumber(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Text, p.Status, p.AccountId, a.FirstName + ' ' + a.LastName As AccountName, p.AssignTimeStamp, f.Id As ImageId, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.AccountId
                            Where b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, BookId = bookId, SequenceNumber = sequenceNumber },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<BookPageModel>(command);
            }
        }

        public async Task DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete p From BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<BookPageModel> UpdatePage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, PageStatuses status, int? chapterId, int? accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update p
                            SET p.Text = @Text, p.ImageId = @ImageId, Status = @Status, AccountId = @AccountId, ChapterId = @ChapterId
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
                    AccountId = accountId,
                    ChapterId = chapterId
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
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
            using (var connection = _connectionProvider.GetConnection())
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

        public async Task<Page<BookPageModel>> GetPagesByBook(int libraryId, int bookId, int pageNumber, int pageSize, PageStatuses status, AssignmentFilter assignmentFilter, int? assignedTo, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, p.AccountId, a.FirstName + ' ' + a.LastName As AccountName, p.AssignTimeStamp,f.Id As ImageId, p.Text, p.ChapterId, c.Title As ChapterTitle
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Chapter] c ON c.Id = p.ChapterId
                            INNER JOIN Book b ON b.Id = p.BookId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.AccountId
                            WHERE b.LibraryId = @LibraryId AND p.BookId = @BookId
                            AND (@Status = -1 OR p.Status = @Status )
                            AND (
                                ( @AssignmentFilter = 0 ) OR
                                ( @AssignmentFilter = 1 AND p.AccountId IS NOT NULL) OR
                                ( @AssignmentFilter = 2 AND p.AccountId IS NULL) OR
                                ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.AccountId = @AccountId )
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
                                    ( @AssignmentFilter = 1 AND p.AccountId IS NOT NULL) OR
                                    ( @AssignmentFilter = 2 AND p.AccountId IS NULL) OR
                                    ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.AccountId = @AccountId )
                                )";
                var commandCount = new CommandDefinition(sqlCount, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    Status = status,
                    AssignmentFilter = assignmentFilter,
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

        public async Task<BookPageModel> UpdatePageAssignment(int libraryId, int bookId, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update p
                            SET p.AccountId = @AccountId, p.AssignTimeStamp = GETUTCDATE()
                            FROM BookPage p
                            INNER JOIN Book b ON b.Id = p.BookId
                            Where b.LibraryId = @LibraryId AND p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = assignedAccountId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<int> GetLastPageNumberForBook(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, p.AccountId, p.AssignTimeStamp, f.Id As ImageId, p.Text, p.ChapterId, c.Title As ChapterTitle
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

        public async Task<IEnumerable<BookPageModel>> GetPagesByBookChapter(int libraryId, int bookId, int chapterId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Status, p.AccountId, p.AssignTimeStamp, f.Id As ImageId, p.Text, p.ChapterId, c.Title As ChapterTitle
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
    }
}
