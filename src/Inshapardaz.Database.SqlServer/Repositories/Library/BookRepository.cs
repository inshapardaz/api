using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class BookRepository : IBookRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public BookRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<BookModel> AddBook(int libraryId, BookModel book, int? AccountId, CancellationToken cancellationToken)
        {
            int bookId;
            using (var connection = _connectionProvider.GetConnection())
            {
                book.LibraryId = libraryId;
                var sql = @"Insert Into Book
                            (Title, [Description], ImageId, LibraryId, IsPublic, IsPublished, [Language], [Status], SeriesId, SeriesIndex, CopyRights, YearPublished, DateAdded, DateUpdated)
                            OUTPUT Inserted.Id VALUES(@Title, @Description, @ImageId, @LibraryId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @CopyRights, @YearPublished, GETDATE(), GETDATE());";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                bookId = await connection.ExecuteScalarAsync<int>(command);

                await connection.ExecuteAsync(new CommandDefinition(
                    "Delete From BookCategory Where BookId = @BookId",
                    new { BookId = book.Id },
                    cancellationToken: cancellationToken));

                var sqlAuthor = @"Insert Into BookAuthor (BookId, AuthorId) Values (@BookId, @AuthorId);";

                if (book.Authors != null && book.Authors.Any())
                {
                    var bookAuthors = book.Authors.Select(a => new { BookId = bookId, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }

                await connection.ExecuteAsync(new CommandDefinition(
                    "Delete From BookCategory Where BookId = @BookId",
                    new { BookId = book.Id },
                    cancellationToken: cancellationToken));

                var sqlCategory = @"Insert Into BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Book SET
                            Title = @Title, [Description] = @Description,
                            IsPublic = @IsPublic, IsPublished = @IsPublished,
                            [Language] = @Language, [Status] = @Status, SeriesId = @SeriesId,
                            SeriesIndex = @SeriesIndex, CopyRights = @CopyRights,
                            YearPublished = @YearPublished, DateUpdated = GETDATE()
                            Where LibraryId = @LibraryId And Id = @Id";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                await connection.ExecuteAsync(new CommandDefinition(
                                    "Delete From BookAuthor Where BookId = @BookId",
                                    new { BookId = book.Id },
                                    cancellationToken: cancellationToken));

                var sqlAuthor = @"Insert Into BookAuthor (BookId, AuthorId) Values (@BookId, @AuthorId);";

                if (book.Authors != null && book.Authors.Any())
                {
                    var bookAuthors = book.Authors.Select(a => new { BookId = book.Id, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }

                await connection.ExecuteAsync(new CommandDefinition(
                    "Delete From BookCategory Where BookId = @BookId",
                    new { BookId = book.Id },
                    cancellationToken: cancellationToken));

                var sqlCategory = @"Insert Into BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Book Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var param = new
                {
                    LibraryId = libraryId,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                var sql = @"Select Id 
                            From Book
                            Where LibraryId = @LibraryId  
                            ORDER BY Id
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                var bookIds = await connection.QueryAsync(command);

                var sqlCount = @"Select Count(*)
                            From Book
                            Where LibraryId = @LibraryId  ";
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
            using (var connection = _connectionProvider.GetConnection())
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
                    PageNumber = pageNumber,
                    AccountId = AccountId,
                    AuthorFilter = filter.AuthorId,
                    SeriesFilter = filter.SeriesId,
                    CategoryFilter = filter.CategoryId,
                    FavoriteFilter = filter.Favorite,
                    RecentFilter = filter.Read,
                    StatusFilter = filter.Status
                };
                var sql = @"Select b.Id, b.Title, b.seriesIndex, b.DateAdded, r.DateRead
                            From Book b
                            LEFT JOIN Series s On b.SeriesId = s.id
                            LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT JOIN Category c ON bc.CategoryId = c.Id
                            LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id AND fb.AccountId = @AccountId
                            LEFT JOIN RecentBooks r On r.BookId = b.Id AND r.AccountId = @AccountId
                            Where b.LibraryId = @LibraryId
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            GROUP BY b.Id, b.Title, b.seriesIndex, b.DateAdded, r.DateRead " +
                            $" ORDER BY {sortByQuery} {sortDirection} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                var bookIds = await connection.QueryAsync(command);

                var sqlCount = @"SELECT Count(*) FROM (Select b.Id
                            From Book b
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT OUTER JOIN Series s On b.SeriesId = s.id
                            LEFT OUTER JOIN FavoriteBooks f On b.Id = f.BookId
                            LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                            LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id AND fb.AccountId = @AccountId
                            LEFT JOIN RecentBooks r On r.BookId = b.Id AND r.AccountId = @AccountId
                            Where b.LibraryId = @LibraryId
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sortByQuery = $"b.{GetSortByQuery(sortBy)}";
                var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";
                var param = new
                {
                    LibraryId = libraryId,
                    Query = $"%{searchText}%",
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    AccountId = AccountId,
                    AuthorFilter = filter.AuthorId,
                    SeriesFilter = filter.SeriesId,
                    CategoryFilter = filter.CategoryId,
                    FavoriteFilter = filter.Favorite,
                    RecentFilter = filter.Read,
                    StatusFilter = filter.Status
                };

                var sql = @"Select b.Id, b.Title, b.seriesIndex, b.DateAdded
                            From Book b
                            LEFT JOIN Series s On b.SeriesId = s.id
                            LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT JOIN Category c ON bc.CategoryId = c.Id
                            LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id
                            LEFT JOIN RecentBooks r On b.Id = r.BookId
                            Where b.LibraryId = @LibraryId
                            AND b.Title Like @Query
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            GROUP BY b.Id, b.Title, b.seriesIndex, b.DateAdded " +
                            $" ORDER BY {sortByQuery} {sortDirection} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                var bookIds = await connection.QueryAsync(command);

                var sqlCount = @"SELECT Count(*) FROM (Select b.Id
                            From Book b
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT OUTER JOIN Series s On b.SeriesId = s.id
                            LEFT OUTER JOIN FavoriteBooks f On b.Id = f.BookId
                            LEFT OUTER JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT OUTER JOIN Category c ON bc.CategoryId = c.Id
                            LEFT OUTER JOIN FavoriteBooks fb On fb.BookId = b.Id
                            LEFT OUTER JOIN RecentBooks r On b.Id = r.BookId
                            Where b.LibraryId = @LibraryId
                            AND b.Title Like @Query
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
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

        public async Task<Page<BookModel>> GetBooksByUser(int libraryId, int accountId, int pageNumber, int pageSize, BookStatuses status, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sortByQuery = $"b.{GetSortByQuery(sortBy)}";
                var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";
                var assignmentfilter = string.Empty;
                if (status == BookStatuses.BeingTyped)
                {
                    assignmentfilter = "AND (bp.WriterAccountId = @AccountId OR c.WriterAccountId = @AccountId)";
                }
                else if (status == BookStatuses.ProofRead)
                {
                    assignmentfilter = "AND (bp.ReviewerAccountId = @AccountId OR c.ReviewerAccountId = @AccountId)";
                }
                var param = new
                {
                    LibraryId = libraryId,
                    PageSize = pageSize,
                    PageNumber = pageNumber,
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
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

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
            using (var connection = _connectionProvider.GetConnection())
            {
                BookModel book = null;
                var sql = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            a.*, c.*
                            from Book b
                            Left Outer Join BookAuthor ba ON b.Id = ba.BookId
                            Left Outer Join Author a On ba.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId AND (f.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            LEFT OUTER JOIN [File] fl ON fl.Id = b.ImageId
                            Where b.LibraryId = @LibraryId AND b.Id = @Id";
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

        public async Task AddRecentBook(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                // TODO :  Delete to old records
                var sql = @"Delete From RecentBooks Where LibraryId = @LibraryId And BookId = @BookId And AccountId = @AccountId;
                            Insert Into RecentBooks (BookId, AccountId, DateRead, LibraryId) VALUES (@BookId, @AccountId, GETDATE(), @LibraryId);";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, AccountId = AccountId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteBookFromRecent(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From RecentBooks Where LibraryId = @LibraryId And BookId = @BookId And AccountId = @AccountId;";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, AccountId = AccountId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddBookToFavorites(int libraryId, int? AccountId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var check = "Select count(*) From FavoriteBooks Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId;";
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From FavoriteBooks Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = AccountId, BookId = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete bc
                            From BookContent bc
                            Inner Join Book b On b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            Where b.LibraryId = @LibraryId and b.Id = @BookId And f.MimeType = @MimeType AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Language = language, MimeType = mimeType, BookId = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl
                            FROM BookContent bc
                            INNER JOIN Book b ON b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND bc.BookId = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<BookContentModel>(command);
            }
        }

        public async Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl
                            FROM BookContent bc
                            INNER JOIN Book b ON b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND bc.BookId = @BookId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<BookContentModel>(command);
            }
        }

        public async Task UpdateBookContentUrl(int libraryId, int bookId, string language, string mimeType, string contentUrl, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update f SET FilePath = @ContentUrl
                            From  [File] f
                            Inner Join BookContent bc On bc.FileId = f.Id
                            Inner Join Book b On b.Id = bc.BookId
                            Where b.LibraryId = @LibraryId and b.Id = @BookId And f.MimeType  = @MimeType AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    BookId = bookId,
                    Language = language,
                    MimeType = mimeType,
                    ContentUrl = contentUrl
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddBookContent(int bookId, int fileId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into BookContent (BookId, FileId, Language)
                            Values (@BookId, @FileId, @Language)";
                var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId, Language = language }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task UpdateBookImage(int libraryId, int bookId, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Book
                            Set ImageId = @ImageId
                            Where Id = @BookId And LibraryId = @LibraryId;";
                var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, LibraryId = libraryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<BookPageSummaryModel>> GetBookPageSummary(int libraryId, IEnumerable<int> bookIds, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var bookSummaries = new Dictionary<int, BookPageSummaryModel>();
                const string sql = @"Select bp.BookId, bp.[Status], Count(*),
                                (Count(bp.Status)* 100 / (Select Count(*) From BookPage WHERE BookPage.BookId = bp.BookId)) as Percentage
                                FROM BookPage bp
                                INNER Join Book b ON b.id = bp.BookId
                                Where b.LibraryId = @LibraryId
                                AND b.Id IN @BookIds
                                AND b.Status <> 0
                                GROUP By bp.BookId, bp.[Status]";

                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookIds = bookIds }, cancellationToken: cancellationToken);
                var results = await connection.QueryAsync<(int BookId, EditingStatus Status, int Count, decimal Percentage)>(command);

                foreach (var result in results)
                {
                    var pageSummary = new PageSummaryModel { Status = result.Status, Count = result.Count, Percentage = result.Percentage };
                    if (!bookSummaries.TryGetValue(result.BookId, out BookPageSummaryModel bookSummary))
                    {
                        bookSummaries.Add(result.BookId, new BookPageSummaryModel
                        {
                            BookId = result.BookId,
                            Statuses = new List<PageSummaryModel> { pageSummary }
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
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            a.*, c.*
                            From Book b
                            LEFT JOIN Series s On b.SeriesId = s.id
                            LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT JOIN Category c ON bc.CategoryId = c.Id
                            LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id
                            LEFT JOIN RecentBooks r On b.Id = r.BookId
                            LEFT OUTER JOIN [File] fl ON fl.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
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
                    return "seriesIndex";

                default:
                    return "Title";
            }
        }
    }
}
