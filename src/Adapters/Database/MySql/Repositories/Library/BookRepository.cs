﻿using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class BookRepository : IBookRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public BookRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<BookModel> AddBook(int libraryId, BookModel book, int? AccountId, CancellationToken cancellationToken)
    {
        int bookId;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            book.LibraryId = libraryId;
            var sql = @"INSERT INTO Book (Title, `Description`, Publisher, Source, ImageId, LibraryId, IsPublic, IsPublished, `Language`, `Status`, SeriesId, SeriesIndex, CopyRights, YearPublished, DateAdded, DateUpdated) 
                            VALUES(@Title, @Description, @Publisher, @Source, @ImageId, @LibraryId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @CopyRights, @YearPublished, UTC_TIMESTAMP(), UTC_TIMESTAMP());
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
            bookId = await connection.ExecuteScalarAsync<int>(command);

            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM BookCategory WHERE BookId = @BookId",
                new { BookId = book.Id },
                cancellationToken: cancellationToken));

            var sqlAuthor = @"INSERT INTO BookAuthor (BookId, AuthorId) 
                                  VALUES (@BookId, @AuthorId);";

            if (book.Authors != null && book.Authors.Any())
            {
                var bookAuthors = book.Authors.Select(a => new { BookId = bookId, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM BookCategory WHERE BookId = @BookId",
                new { BookId = book.Id },
                cancellationToken: cancellationToken));

            var sqlCategory = @"INSERT INTO BookCategory (BookId, CategoryId) 
                                    VALUES (@BookId, @CategoryId);";

            if (book.Categories != null && book.Categories.Any())
            {
                var bookCategories = book.Categories.Select(c => new { BookId = bookId, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, bookCategories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            return await GetBookById(libraryId, bookId, AccountId, cancellationToken);
        }
    }

    public async Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken)
    {
        book.LibraryId = libraryId;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Book 
                        SET
                            Title = @Title, `Description` = @Description,
                            Publisher = @Publisher, Source = @Source, 
                            IsPublic = @IsPublic, IsPublished = @IsPublished,
                            `Language` = @Language, `Status` = @Status, SeriesId = @SeriesId,
                            SeriesIndex = @SeriesIndex, CopyRights = @CopyRights,
                            YearPublished = @YearPublished, DateUpdated = UTC_TIMESTAMP()
                        WHERE LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);

            await connection.ExecuteAsync(new CommandDefinition(
                                "DELETE FROM BookAuthor WHERE BookId = @BookId",
                                new { BookId = book.Id },
                                cancellationToken: cancellationToken));

            var sqlAuthor = @"INSERT INTO BookAuthor (BookId, AuthorId) 
                                  VALUES (@BookId, @AuthorId);";

            if (book.Authors != null && book.Authors.Any())
            {
                var bookAuthors = book.Authors.Select(a => new { BookId = book.Id, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM BookCategory WHERE BookId = @BookId",
                new { BookId = book.Id },
                cancellationToken: cancellationToken));

            var sqlCategory = @"INSERT INTO BookCategory (BookId, CategoryId) VALUES (@BookId, @CategoryId);";

            if (book.Categories != null && book.Categories.Any())
            {
                var bookCategories = book.Categories.Select(c => new { BookId = book.Id, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, bookCategories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }
        }
    }

    public async Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM Book WHERE LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var param = new
            {
                LibraryId = libraryId,
                PageSize = pageSize,
                Offset = pageSize * (pageNumber - 1)
            };
            var sql = @"SELECT Id 
                        FROM Book
                        WHERE LibraryId = @LibraryId  
                        ORDER BY Id
                        LIMIT @PageSize
                        OFFSET @Offset";
            var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

            var bookIds = await connection.QueryAsync(command);

            var sqlCount = @"SELECT Count(*)
                            FROM Book
                            WHERE LibraryId = @LibraryId";
            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken);

            return new Page<BookModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookCount,
                Data = books
            };
        }
    }

    public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, int? AccountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sortByQuery = $"b.{GetSortByQuery(sortBy)}";
            var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";

            if (filter.Read.HasValue && filter.Read.Value)
            {
                sortByQuery = "r.DateRead";
            }

            var param = new
            {
                LibraryId = libraryId,
                PageSize = pageSize,
                Offset = pageSize * (pageNumber - 1),
                AccountId = AccountId,
                AuthorFilter = filter.AuthorId,
                SeriesFilter = filter.SeriesId,
                CategoryFilter = filter.CategoryId,
                FavoriteFilter = filter.Favorite,
                RecentFilter = filter.Read,
                StatusFilter = filter.Status
            };
            var sql = @"SELECT b.Id, b.Title, b.seriesIndex, b.DateAdded, r.DateRead
                            FROM Book b
                                LEFT JOIN Series s On b.SeriesId = s.id
                                LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                                INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                                INNER JOIN Author a On ba.AuthorId = a.Id
                                LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT JOIN Category c ON bc.CategoryId = c.Id
                                LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id AND fb.AccountId = @AccountId
                                LEFT JOIN RecentBooks r On r.BookId = b.Id AND r.AccountId = @AccountId
                            WHERE b.LibraryId = @LibraryId
                                AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                                AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                                AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                                AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                                AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                                AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                                AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            GROUP BY b.Id, b.Title, b.seriesIndex, b.DateAdded, r.DateRead " +
                        $" ORDER BY {sortByQuery} {sortDirection} " +
                        "LIMIT @PageSize OFFSET @Offset";
            var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

            var bookIds = await connection.QueryAsync(command);

            var sqlCount = @"SELECT Count(*) FROM (Select b.Id
                            FROM Book b
                                INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                                INNER JOIN Author a On ba.AuthorId = a.Id
                                LEFT OUTER JOIN Series s On b.SeriesId = s.id
                                LEFT OUTER JOIN FavoriteBooks f On b.Id = f.BookId
                                LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                                LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id AND fb.AccountId = @AccountId
                                LEFT JOIN RecentBooks r On r.BookId = b.Id AND r.AccountId = @AccountId
                            WHERE b.LibraryId = @LibraryId
                                AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                                AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                                AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                                AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                                AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                                AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                                AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            GROUP BY b.Id) AS bkcnt";
            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken);

            return new Page<BookModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookCount,
                Data = books
            };
        }
    }

    public async Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, int? AccountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sortByQuery = $"b.{GetSortByQuery(sortBy)}";
            var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";
            var param = new
            {
                LibraryId = libraryId,
                Query = $"%{searchText}%",
                PageSize = pageSize,
                Offset = pageSize * (pageNumber - 1),
                AccountId = AccountId,
                AuthorFilter = filter.AuthorId,
                SeriesFilter = filter.SeriesId,
                CategoryFilter = filter.CategoryId,
                FavoriteFilter = filter.Favorite,
                RecentFilter = filter.Read,
                StatusFilter = filter.Status
            };

            var sql = @"SELECT b.Id, b.Title, b.seriesIndex, b.DateAdded
                            FROM Book b
                                LEFT JOIN Series s On b.SeriesId = s.id
                                LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                                INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                                INNER JOIN Author a On ba.AuthorId = a.Id
                                LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT JOIN Category c ON bc.CategoryId = c.Id
                                LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id
                                LEFT JOIN RecentBooks r On b.Id = r.BookId
                            WHERE b.LibraryId = @LibraryId
                                AND b.Title Like @Query
                                AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                                AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                                AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                                AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                                AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                                AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                                AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            GROUP BY b.Id, b.Title, b.seriesIndex, b.DateAdded " +
                        $" ORDER BY {sortByQuery} {sortDirection} " +
                        @"LIMIT @PageSize OFFSET @Offset";

            var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

            var bookIds = await connection.QueryAsync(command);

            var sqlCount = @"SELECT Count(*) FROM (Select b.Id
                            FROM Book b
                                INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                                INNER JOIN Author a On ba.AuthorId = a.Id
                                LEFT OUTER JOIN Series s On b.SeriesId = s.id
                                LEFT OUTER JOIN FavoriteBooks f On b.Id = f.BookId
                                LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                                LEFT OUTER JOIN FavoriteBooks fb On fb.BookId = b.Id
                                LEFT OUTER JOIN RecentBooks r On b.Id = r.BookId
                            WHERE b.LibraryId = @LibraryId
                                AND b.Title Like @Query
                                AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                                AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                                AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                                AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                                AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                                AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                                AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            GROUP BY b.Id) AS bkcnt";

            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken);

            return new Page<BookModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookCount,
                Data = books
            };
        }
    }

    public async Task<Page<BookModel>> GetBooksByUser(int libraryId, int accountId, int pageNumber, int pageSize, StatusType status, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sortByQuery = $"b.{GetSortByQuery(sortBy)}";
            var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";
            var assignmentfilter = string.Empty;
            if (status == StatusType.BeingTyped)
            {
                assignmentfilter = "AND (bp.WriterAccountId = @AccountId OR c.WriterAccountId = @AccountId)";
            }
            else if (status == StatusType.ProofRead)
            {
                assignmentfilter = "AND (bp.ReviewerAccountId = @AccountId OR c.ReviewerAccountId = @AccountId)";
            }
            var param = new
            {
                LibraryId = libraryId,
                PageSize = pageSize,
                Offset = pageSize * (pageNumber - 1),
                AccountId = accountId,
                StatusFilter = status
            };

            var sql = @"SELECT DISTINCT b.Id, b.Title
                            FROM Book b
                                LEFT JOIN BookPage bp ON bp.BookId = b.Id
                                LEFT JOIN Chapter c ON c.BookId = b.Id
                            WHERE b.LibraryId = @LibraryId
                                AND b.Status = @StatusFilter " +
                        assignmentfilter +
                        $" ORDER BY {sortByQuery}, b.Id {sortDirection} " +
                        @"LIMIT @PageSize OFFSET @Offset";

            var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

            var bookIds = await connection.QueryAsync(command);

            var sqlCount = @"SELECT COUNT(DISTINCT b.Id)
                            FROM Book b
                                LEFT JOIN BookPage bp ON bp.BookId = b.Id
                                LEFT JOIN Chapter c ON c.BookId = b.Id
                            WHERE b.LibraryId = @LibraryId
                                AND b.Status = @StatusFilter " +
                        assignmentfilter;

            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken);

            return new Page<BookModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookCount,
                Data = books
            };
        }
    }

    public async Task<BookModel> GetBookById(int libraryId, int bookId, int? AccountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            BookModel book = null;
            var sql = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                                CASE WHEN fb.BookId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                                (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                                (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.id) As ChapterCount,
                                a.*, c.*
                            FROM Book b
                                LEFT OUTER JOIN BookAuthor ba ON b.Id = ba.BookId
                                LEFT OUTER JOIN Author a On ba.AuthorId = a.Id
                                LEFT OUTER JOIN Series s On b.SeriesId = s.id
                                LEFT OUTER JOIN FavoriteBooks f On b.Id = f.BookId AND (f.AccountId = @AccountId OR @AccountId Is Null)
                                LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                                LEFT OUTER JOIN FavoriteBooks fb On fb.BookId = b.Id
                                LEFT OUTER JOIN `File` fl ON fl.Id = b.ImageId
                            WHERE b.LibraryId = @LibraryId 
                                AND b.Id = @Id";
            await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, BookModel>(sql, (b, a, c) =>
            {
                if (book == null)
                {
                    book = b;
                }

                if (!book.Authors.Any(x => x.Id == a.Id))
                {
                    book.Authors.Add(a);
                }

                if (c != null && !book.Categories.Any(x => x.Id == c.Id))
                {
                    book.Categories.Add(c);
                }

                return book;
            }, new { LibraryId = libraryId, Id = bookId, AccountId = AccountId });

            return book;
        }
    }

    public async Task<BookModel> GetBookBySource(int libraryId, string source, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            BookModel book = null;
            var sql = @"SELECT b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                                (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                                (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.id) As ChapterCount,
                                a.*, c.*
                            FROM Book b
                                LEFT OUTER JOIN BookAuthor ba ON b.Id = ba.BookId
                                LEFT OUTER JOIN Author a On ba.AuthorId = a.Id
                                LEFT OUTER JOIN Series s On b.SeriesId = s.id
                                LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                                LEFT OUTER JOIN `File` fl ON fl.Id = b.ImageId
                            WHERE b.LibraryId = @LibraryId 
                                AND b.Source= @Source";
            await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, BookModel>(sql, (b, a, c) =>
            {
                if (book == null)
                {
                    book = b;
                }

                if (!book.Authors.Any(x => x.Id == a.Id))
                {
                    book.Authors.Add(a);
                }

                if (c != null && !book.Categories.Any(x => x.Id == c.Id))
                {
                    book.Categories.Add(c);
                }

                return book;
            }, new { LibraryId = libraryId, Source = source });

            return book;
        }
    }

    public async Task<BookModel> GetBookByPublisher(int libraryId, string publisher, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            BookModel book = null;
            var sql = @"SELECT b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                                (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                                (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.id) As ChapterCount,
                                a.*, c.*
                            FROM Book b
                                LEFT OUTER JOIN BookAuthor ba ON b.Id = ba.BookId
                                LEFT OUTER JOIN Author a On ba.AuthorId = a.Id
                                LEFT OUTER JOIN Series s On b.SeriesId = s.id
                                LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                                LEFT OUTER JOIN `File` fl ON fl.Id = b.ImageId
                            WHERE b.LibraryId = @LibraryId 
                                AND b.Publisher= @Publisher";
            await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, BookModel>(sql, (b, a, c) =>
            {
                if (book == null)
                {
                    book = b;
                }

                if (!book.Authors.Any(x => x.Id == a.Id))
                {
                    book.Authors.Add(a);
                }

                if (c != null && !book.Categories.Any(x => x.Id == c.Id))
                {
                    book.Categories.Add(c);
                }

                return book;
            }, new { LibraryId = libraryId, Publisher = publisher });

            return book;
        }
    }

    public async Task AddRecentBook(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            // TODO :  Delete to old records
            var sql = @"DELETE FROM RecentBooks WHERE LibraryId = @LibraryId AND BookId = @BookId AND AccountId = @AccountId;
                            INSERT INTO RecentBooks (BookId, AccountId, DateRead, LibraryId) 
                            VALUES (@BookId, @AccountId, UTC_TIMESTAMP(), @LibraryId);";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, AccountId = AccountId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteBookFromRecent(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM RecentBooks WHERE LibraryId = @LibraryId AND BookId = @BookId AND AccountId = @AccountId;";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, AccountId = AccountId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task AddBookToFavorites(int libraryId, int? AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var check = "SELECT COUNT(*) FROM FavoriteBooks WHERE LibraryId = @LibraryId AND AccountId = @AccountId AND BookId = @BookId;";
            var commandCheck = new CommandDefinition(check, new { LibraryId = libraryId, AccountId = AccountId, BookId = bookId }, cancellationToken: cancellationToken);
            var count = await connection.ExecuteScalarAsync<int>(commandCheck);

            if (count > 0) return;

            var sql = @"INSERT INTO FavoriteBooks (LibraryId, AccountId, BookId, DateAdded) VALUES(@LibraryId, @AccountId, @BookId, @DateAdded);";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = AccountId, BookId = bookId, DateAdded = DateTime.UtcNow }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteBookFromFavorites(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE FROM FavoriteBooks WHERE LibraryId = @LibraryId AND AccountId = @AccountId AND BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = AccountId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteBookContent(int libraryId, int bookId, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE bc
                            FROM BookContent bc
                                INNER JOIN Book b ON b.Id = bc.BookId
                                INNER JOIN `File` f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
                                AND b.Id = @BookId 
                                AND bc.id= @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = contentId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName
                            FROM BookContent bc
                                INNER JOIN Book b ON b.Id = bc.BookId
                                INNER JOIN `File` f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
                                AND bc.BookId = @BookId 
                                AND bc.Language = @Language 
                                AND f.MimeType = @MimeType";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookContentModel>(command);
        }
    }

    public async Task<BookContentModel> GetBookContent(int libraryId, int bookId, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName
                            FROM BookContent bc
                                INNER JOIN Book b ON b.Id = bc.BookId
                                INNER JOIN `File` f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
                                AND bc.BookId = @BookId 
                                AND bc.Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, Id = contentId }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookContentModel>(command);
        }
    }

    public async Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName
                            FROM BookContent bc
                                INNER JOIN Book b ON b.Id = bc.BookId
                                INNER JOIN `File` f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
                                AND bc.BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<BookContentModel>(command);
        }
    }

    public async Task UpdateBookContent(int libraryId, int bookId, int contentId, string language, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE BookContent bc 
                               INNER JOIN Book b ON b.Id = bc.BookId
                            SET bc.Language = @Language
                            WHERE b.LibraryId = @LibraryId 
                                AND b.Id = @BookId 
                                AND bc.Id = @Id";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                Id = contentId,
                Language = language,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<int> AddBookContent(int bookId, long fileId, string language, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO BookContent (BookId, FileId, Language)
                            VALUES (@BookId, @FileId, @Language);
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId, Language = language }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task UpdateBookImage(int libraryId, int bookId, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Book
                            SET ImageId = @ImageId
                            WHERE Id = @BookId 
                                AND LibraryId = @LibraryId;";
            var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, LibraryId = libraryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<PageSummaryModel>> GetBookPageSummary(int libraryId, IEnumerable<int> bookIds, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var bookSummaries = new Dictionary<int, PageSummaryModel>();
            const string sql = @"SELECT bp.BookId, bp.`Status`, Count(*),
                                (Count(bp.Status)* 100 / (Select Count(*) From BookPage WHERE BookPage.BookId = bp.BookId)) as Percentage
                                FROM BookPage bp
                                    INNER Join Book b ON b.id = bp.BookId
                                WHERE b.LibraryId = @LibraryId
                                    AND b.Id IN @BookIds
                                    AND b.Status <> 0
                                GROUP By bp.BookId, bp.`Status`";

            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookIds = bookIds }, cancellationToken: cancellationToken);
            var results = await connection.QueryAsync<(int BookId, EditingStatus Status, int Count, decimal Percentage)>(command);

            foreach (var result in results)
            {
                var pageSummary = new PageStatusSummaryModel { Status = result.Status, Count = result.Count, Percentage = result.Percentage };
                if (!bookSummaries.TryGetValue(result.BookId, out PageSummaryModel bookSummary))
                {
                    bookSummaries.Add(result.BookId, new PageSummaryModel
                    {
                        BookId = result.BookId,
                        Statuses = new List<PageStatusSummaryModel> { pageSummary }
                    });
                }
                else
                {
                    bookSummary.Statuses.Add(pageSummary);
                }
            }

            return bookSummaries.Values;
        }
    }

    private async Task<IEnumerable<BookModel>> GetBooks(IDbConnection connection, int libraryId, List<int> bookIds, CancellationToken cancellationToken)
    {
        var books = new Dictionary<int, BookModel>();
        var sql3 = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                                CASE WHEN fb.BookId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                                (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.Id) As PageCount,
                                (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.Id) As ChapterCount,
                                a.*, c.*
                            FROM Book b
                                LEFT JOIN Series s On b.SeriesId = s.id
                                LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                                INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                                INNER JOIN Author a On ba.AuthorId = a.Id
                                LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                                LEFT JOIN Category c ON bc.CategoryId = c.Id
                                LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id
                                LEFT JOIN RecentBooks r On b.Id = r.BookId
                                LEFT OUTER JOIN `File` fl ON fl.Id = b.ImageId
                            WHERE b.LibraryId = @LibraryId
                                AND b.Id IN @BookList";
        var command3 = new CommandDefinition(sql3, new { LibraryId = libraryId, BookList = bookIds }, cancellationToken: cancellationToken);

        await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, BookModel>(command3, (b, a, c) =>
        {
            if (!books.TryGetValue(b.Id, out BookModel book))
                books.Add(b.Id, book = b);

            if (!book.Authors.Any(x => x.Id == a.Id))
            {
                book.Authors.Add(a);
            }

            if (c != null && !book.Categories.Any(x => x.Id == c.Id))
            {
                book.Categories.Add(c);
            }

            return book;
        });

        return books.Values.OrderBy(b => bookIds.IndexOf(b.Id)).ToList();
    }

    private static string GetSortByQuery(BookSortByType sortBy)
    {
        switch (sortBy)
        {
            case BookSortByType.DateCreated:
                return "DateAdded";

            case BookSortByType.seriesIndex:
                return "SeriesIndex";

            default:
                return "`Title`";
        }
    }
}
