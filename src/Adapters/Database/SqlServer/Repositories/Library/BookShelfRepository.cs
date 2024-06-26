﻿
using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;

public class BookShelfRepository : IBookShelfRepository
{
    private readonly SqlServerConnectionProvider _connectionProvider;

    public BookShelfRepository(SqlServerConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<BookShelfModel> AddBookShelf(int libraryId, BookShelfModel bookShelf, CancellationToken cancellationToken)
    {
        int bookShelfId;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Insert Into BookShelf(Name, Description, ImageId, LibraryId, IsPublic, AccountId) OUTPUT Inserted.Id VALUES(@Name, @Description, @ImageId, @LibraryId, @IsPublic, @AccountId);";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                Name = bookShelf.Name,
                Description = bookShelf.Description,
                ImageId = bookShelf.ImageId,
                IsPublic = bookShelf.IsPublic,
                AccountId = bookShelf.AccountId
            }, cancellationToken: cancellationToken);
            bookShelfId = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetBookShelfById(libraryId, bookShelfId, cancellationToken);
    }

    public async Task UpdateBookShelf(int libraryId, BookShelfModel bookShelf, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update BookShelf Set Name = @Name, Description = @Description, ImageId = @ImageId, IsPublic = @IsPublic, AccountId = @AccountId Where Id = @Id AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new
            {
                Id = bookShelf.Id,
                LibraryId = libraryId,
                Name = bookShelf.Name,
                Description = bookShelf.Description,
                ImageId = bookShelf.ImageId,
                IsPublic = bookShelf.IsPublic,
                AccountId = bookShelf.AccountId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteBookShelf(int libraryId, int bookshelfId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From BookShelf Where LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = bookshelfId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<BookShelfModel> GetBookShelfById(int libraryId, int bookshelfId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT b.Id, b.Name, b.Description, b.IsPublic, b.AccountId, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM BookShelfBook 
                                WHERE BookShelfId = b.Id) AS BookCount
                            FROM BookShelf AS b
                            LEFT OUTER JOIN [File] f ON f.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
                            And b.Id = @BookShelfId";
            var command = new CommandDefinition(sql,
                                                new { LibraryId = libraryId, BookShelfId = bookshelfId },
                                                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<BookShelfModel>(command);
        }
    }

    public async Task<Page<BookShelfModel>> GetBookShelves(int libraryId, bool onlyPublic, int pageNumber, int pageSize, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT b.Id, b.Name, b.Description, b.IsPublic, b.AccountId, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM BookShelfBook 
                                WHERE BookShelfId = b.Id) AS BookCount
                            FROM BookShelf AS b
                            LEFT OUTER JOIN [File] f ON f.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
                            AND b.IsPublic = @Public 
                            AND ((@Public = 1 AND b.AccountId <> @AccountId) 
                                OR (@Public = 0 AND  b.AccountId = @AccountId))
                            Order By b.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PageSize = pageSize,
                                                    PageNumber = pageNumber,
                                                    AccountId = accountId,
                                                    Public = onlyPublic
                                                },
                                                cancellationToken: cancellationToken);

            var bookShelves = await connection.QueryAsync<BookShelfModel>(command);

            var sqlShelfCount = @"SELECT COUNT(*) 
                    FROM BookShelf 
                    WHERE LibraryId = @LibraryId 
                    AND IsPublic = @Public 
                    AND ((@Public = 1 AND AccountId <> @AccountId) 
                        OR (@Public = 0 AND  AccountId = @AccountId))";
            var bookShelfCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlShelfCount,
                new
                {
                    LibraryId = libraryId,
                    AccountId = accountId,
                    Public = onlyPublic
                }, cancellationToken: cancellationToken));

            return new Page<BookShelfModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookShelfCount,
                Data = bookShelves
            };
        }
    }

    public async Task<Page<BookShelfModel>> FindBookShelves(int libraryId, string query, bool onlyPublic, int pageNumber, int pageSize, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT b.Id, b.Name, b.Description, b.IsPublic, b.AccountId, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM BookShelfBook 
                                WHERE BookShelfId = b.Id) AS BookCount
                            FROM BookShelf AS b
                            LEFT OUTER JOIN [File] f ON f.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
                            AND b.IsPublic = @Public 
                            AND ((@Public = 1 AND b.AccountId <> @AccountId) 
                                OR (@Public = 0 AND  b.AccountId = @AccountId))
                            AND b.Name LIKE @Query 
                            Order By b.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    Query = $"%{query}%",
                                                    PageSize = pageSize,
                                                    PageNumber = pageNumber,
                                                    AccountId = accountId,
                                                    Public = onlyPublic
                                                },
                                                cancellationToken: cancellationToken);

            var bookShelves = await connection.QueryAsync<BookShelfModel>(command);

            var sqlBookShelfCount = @"SELECT COUNT(*)
                    FROM BookShelf
                    WHERE LibraryId = @LibraryId
                    AND IsPublic = @Public
                    AND ((@Public = 1 AND AccountId <> @AccountId) 
                        OR (@Public = 0 AND  AccountId = @AccountId))
                    AND Name LIKE @Query";
            var bookShelfCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlBookShelfCount, new
            {
                LibraryId = libraryId,
                Query = $"%{query}%",
                AccountId = accountId,
                Public = onlyPublic
            }, cancellationToken: cancellationToken));

            return new Page<BookShelfModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookShelfCount,
                Data = bookShelves
            };
        }
    }

    public async Task<Page<BookShelfModel>> GetAllBookShelves(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT b.Id, b.Name, b.Description, b.IsPublic, b.AccountId, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM BookShelfBook 
                                WHERE BookShelfId = b.Id) AS BookCount
                            FROM BookShelf AS b
                            LEFT OUTER JOIN [File] f ON f.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
                            Order By b.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PageSize = pageSize,
                                                    PageNumber = pageNumber
                                                },
                                                cancellationToken: cancellationToken);

            var bookShelves = await connection.QueryAsync<BookShelfModel>(command);

            var sqlShelfCount = @"SELECT COUNT(*) 
                    FROM BookShelf 
                    WHERE LibraryId = @LibraryId";
            var bookShelfCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlShelfCount,
                new
                {
                    LibraryId = libraryId
                }, cancellationToken: cancellationToken));

            return new Page<BookShelfModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookShelfCount,
                Data = bookShelves
            };
        }
    }

    public async Task AddBookToBookShelf(int libraryId, int bookshelfId, int bookId, int index, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO BookShelfBook (BookId, BookShelfId, [Index]) 
                            VALUES (@BookId, @BookShelfId, @Index)";
            var command = new CommandDefinition(sql, new
            {
                BookId = bookId,
                BookShelfId = bookshelfId,
                Index = index
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task UpdateBookToBookShelf(int libraryId, BookShelfBook bookShelfBook, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE BookShelfBook SET [Index] = @Index 
                            WHERE BookId = @BookId AND BookShelfId = @BookShelfId";
            var command = new CommandDefinition(sql, new
            {
                BookId = bookShelfBook.BookId,
                BookShelfId = bookShelfBook.BookShelfId,
                Index = bookShelfBook.Index
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task RemoveBookFromBookShelf(int libraryId, int bookshelfId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM BookShelfBook WHERE BookId = @BookId AND BookShelfId = @BookShelfId";
            var command = new CommandDefinition(sql, new
            {
                BookId = bookId,
                BookShelfId = bookshelfId,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task UpdateBookShelfImage(int libraryId, int bookshelfId, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update BookShelf Set ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId";
            var command = new CommandDefinition(sql, new { Id = bookshelfId, LibraryId = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task<IEnumerable<BookShelfBook>> GetBookShelfBooks(int libraryId, int bookShelfId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT * FROM BookShelfBook
                            WHERE BookShelfId = @BookShelfId";
            var command = new CommandDefinition(sql, new { BookShelfId = bookShelfId }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<BookShelfBook>(command);
        }
    }

    public async Task<BookShelfBook> GetBookFromBookShelfById(int libraryId, int bookShelfId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT * FROM BookShelfBook
                            WHERE BookShelfId = @BookShelfId
                            AND BookId = @BookId";
            var command = new CommandDefinition(sql, new { BookShelfId = bookShelfId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookShelfBook>(command);
        }
    }

}
