using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
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
                            (Title, [Description], AuthorId, ImageId, LibraryId, IsPublic, IsPublished, [Language], [Status], SeriesId, SeriesIndex, CopyRights, YearPublished, DateAdded, DateUpdated)
                            OUTPUT Inserted.Id VALUES(@Title, @Description, @AuthorId, @ImageId, @LibraryId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @CopyRights, @YearPublished, GETDATE(), GETDATE());";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                bookId = await connection.ExecuteScalarAsync<int>(command);

                var sqlCategory = @"Delete From BookCategory Where BookId = @BookId;
                           Insert Into BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

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
                            AuthorId = @AuthorId, IsPublic = @IsPublic, IsPublished = @IsPublished,
                            [Language] = @Language, [Status] = @Status, SeriesId = @SeriesId,
                            SeriesIndex = @SeriesIndex, CopyRights = @CopyRights,
                            YearPublished = @YearPublished, DateUpdated = GETDATE()
                            Where LibraryId = @LibraryId And Id = @Id";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                var sqlCategory = @"Delete From BookCategory Where BookId = @BookId AND CategoryId = @CategoryId;
                           Insert Into BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

                var bookCategories = book.Categories.Select(c => new { BookId = book.Id, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, bookCategories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
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

        public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, int? AccountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();
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
                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            c.*
                            From Book b
                            Inner Join Author a On b.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            Left Outer Join RecentBooks r On b.Id = r.BookId
                            Where b.LibraryId = @LibraryId
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
                            AND (a.Id = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL) " +
                            $" ORDER BY {sortByQuery} {sortDirection} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    if (c != null)
                    {
                        book.Categories.Add(c);
                    }

                    return book;
                });

                var sqlCount = @"Select Distinct Count(b.Id)
                            From Book b
                            Inner Join Author a On b.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            Left Outer Join RecentBooks r On b.Id = r.BookId
                            Where b.LibraryId = @LibraryId
                            AND (@AccountId IS NOT NULL OR b.IsPublic = 1)
                            AND (b.Status = @StatusFilter OR @StatusFilter IS NULL)
                            AND (a.Id = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL) ";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
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

        public async Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, int? AccountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();
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
                    FavoriteFilter = filter.Favorite
                };
                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            c.*
                            From Book b
                            Inner Join Author a On b.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            Where b.LibraryId = @LibraryId And b.Title Like @Query AND (@AccountId Is not Null OR b.IsPublic = 1)
                            AND (a.Id = @AuthorFilter OR @AuthorFilter Is Null)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter Is Null)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter Is Null)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS Null) " +
                            $" ORDER BY {sortByQuery} {sortDirection} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    if (c != null)
                    {
                        book.Categories.Add(c);
                    }

                    return book;
                });

                var sqlCount = @"SELECT COUNT(*)
                                From Book b
                                Inner Join Author a On b.AuthorId = a.Id
                                Left Outer Join Series s On b.SeriesId = s.id
                                Left Outer Join FavoriteBooks f On b.Id = f.BookId
                                Left Outer Join BookCategory bc ON b.Id = bc.BookId
                                Left Outer Join Category c ON bc.CategoryId = c.Id
                                Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                                Where b.LibraryId = @LibraryId And b.Title Like @Query AND(@AccountId Is not Null OR b.IsPublic = 1)
                                AND (a.Id = @AuthorFilter OR @AuthorFilter Is Null)
                                AND (s.Id = @SeriesFilter OR @SeriesFilter Is Null)
                                AND (f.AccountId = @AccountId OR @FavoriteFilter Is Null)
                                AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS Null) ";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<Page<BookModel>> GetLatestBooks(int libraryId, int pageNumber, int pageSize, int? AccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Book b
                            Inner Join Author a On b.AuthorId = a.Id
                            Inner Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId AND (f.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId
                            Order By b.DateAdded
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber, AccountId = AccountId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    if (c != null)
                        book.Categories.Add(c);

                    return book;
                });

                var sqlCount = "SELECT COUNT(*) FROM Books WHERE LibraryId = @LibraryId";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<BookModel> GetBookById(int libraryId, int bookId, int? AccountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                BookModel book = null;
                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            c.*
                            from Book b
                            Inner Join Author a On b.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId AND (f.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            Where b.LibraryId = @LibraryId AND b.Id = @Id";
                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(sql, (b, c) =>
                {
                    if (book == null)
                    {
                        book = b;
                    }

                    if (c != null)
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
                var results = await connection.QueryAsync<(int BookId, PageStatuses Status, int Count, decimal Percentage)>(command);

                foreach (var result in results)
                {
                    var pageSummary = new PageSummaryModel { Status = result.Status, Count = result.Count, Percentage = result.Percentage};
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
    }
}
