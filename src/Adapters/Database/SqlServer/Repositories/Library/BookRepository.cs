using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Data;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;

public class BookRepository(SqlServerConnectionProvider connectionProvider) : IBookRepository
{
    public async Task<BookModel> AddBook(int libraryId, BookModel book, int? AccountId, CancellationToken cancellationToken)
    {
        int bookId;
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            book.LibraryId = libraryId;
            var sql = @"Insert Into Book
                            (Title, [Description], Publisher, Source, ImageId, LibraryId, IsPublic, IsPublished, [Language], [Status], SeriesId, SeriesIndex, CopyRights, YearPublished, DateAdded, DateUpdated)
                            OUTPUT Inserted.Id VALUES(@Title, @Description, @Publisher, @Source, @ImageId, @LibraryId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @CopyRights, @YearPublished, GETDATE(), GETDATE());";
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
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Book SET
                            Title = @Title, [Description] = @Description,
                            Publisher = @Publisher, Source = @Source, 
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
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Book Where LibraryId = @LibraryId AND Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
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

    public async Task<Page<string>> FindPublishers(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var parameters = new
                        {
                            LibraryId = libraryId,
                            Query = $"%{query}%",
                            PageNumber= pageNumber,
                            PageSize = pageSize,
                        };
            
            var sql = @"Select DISTINCT Publisher 
                            From Book
                            Where LibraryId = @LibraryId
                            AND Publisher Like @Query
                            ORDER BY Publisher
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
            
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<string>(command);
            
            var sql2 = @"Select COUNT(DISTINCT Publisher) 
                            FROM Book
                            Where LibraryId = @LibraryId
                            AND Publisher Like @Query";
            
            var command2 = new CommandDefinition(sql2, parameters, cancellationToken: cancellationToken);
            var count = await connection.ExecuteScalarAsync<int>(command2);
            
            return new Page<string>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = result.ToList()
            };
        }
    }

    public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, int? AccountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
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
                StatusFilter = filter.Status,
                LanguageFilter = filter.Language
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
                            AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (b.Language = @LanguageFilter OR @LanguageFilter IS NULL)
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
                            AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (b.Language = @LanguageFilter OR @LanguageFilter IS NULL)
                            GROUP BY b.Id) AS bkcnt";
            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken, AccountId);

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
        using (var connection = connectionProvider.GetLibraryConnection())
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
                StatusFilter = filter.Status,
                LanguageFilter = filter.Language
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
                            AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (b.Language = @LanguageFilter OR @LanguageFilter IS NULL)
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
                            AND (b.Status = @StatusFilter OR @StatusFilter = 0)
                            AND (ba.AuthorId = @AuthorFilter OR @AuthorFilter IS NULL)
                            AND (s.Id = @SeriesFilter OR @SeriesFilter IS NULL)
                            AND (f.AccountId = @AccountId OR @FavoriteFilter IS NULL)
                            AND (r.AccountId = @AccountId OR @RecentFilter IS NULL)
                            AND (bc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                            AND (b.Language = @LanguageFilter OR @LanguageFilter IS NULL)
                            GROUP BY b.Id) AS bkcnt";

            var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken, AccountId);

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
        using (var connection = connectionProvider.GetLibraryConnection())
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

            var books = await GetBooks(connection, libraryId, bookIds.Select(b => (int)b.Id).ToList(), cancellationToken, accountId);

            return new Page<BookModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = bookCount,
                Data = books
            };
        }
    }

    public async Task<BookModel> GetBookById(int libraryId, long bookId, int? AccountId,
        CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            BookModel book = null;
            var sql = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.id) As ChapterCount,
                            a.*, c.*, r.*
                            from Book b
                            Left Outer Join BookAuthor ba ON b.Id = ba.BookId
                            Left Outer Join Author a On ba.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join FavoriteBooks f On b.Id = f.BookId AND (f.AccountId = @AccountId OR @AccountId Is Null)
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            Left Outer Join FavoriteBooks fb On fb.BookId = b.Id
                            Left Outer Join RecentBooks r On b.Id = r.BookId AND (r.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN [File] fl ON fl.Id = b.ImageId
                            Where b.LibraryId = @LibraryId AND b.Id = @Id";
            await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, ReadProgressModel, BookModel>(sql, (b, a, c, r) =>
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

                if (r != null && (r.ProgressType != null || r.ProgressId != 0 || r.DateRead != null))
                {
                    book.ReadProgress = r;
                }

                return book;
            }, new { LibraryId = libraryId, Id = bookId, AccountId = AccountId }, splitOn: "Id,Id,ProgressType");

            return book;
        }
    }

    public async Task<BookModel> GetBookBySource(int libraryId, string source, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            BookModel book = null;
            var sql = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.id) As PageCount,
                            (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.id) As ChapterCount,
                            a.*, c.*
                            from Book b
                            Left Outer Join BookAuthor ba ON b.Id = ba.BookId
                            Left Outer Join Author a On ba.AuthorId = a.Id
                            Left Outer Join Series s On b.SeriesId = s.id
                            Left Outer Join BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Category c ON bc.CategoryId = c.Id
                            LEFT OUTER JOIN [File] fl ON fl.Id = b.ImageId
                            Where b.LibraryId = @LibraryId AND b.Source= @Source";
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

    public async Task<ReadProgressModel> AddRecentBook(int libraryId, int AccountId, int bookId,
        ReadProgressModel progress,
        CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"REPLACE Into RecentBooks (BookId, AccountId, DateRead, LibraryId, ProgressType, ProgressId, ProgressValue)
                            VALUES (@BookId, @AccountId, GETDATE(), @LibraryId, @ProgressType, @ProgressId, @ProgressValue);
                        SELECT LAST_INSERT_ID()";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId, 
                BookId = bookId, 
                AccountId = AccountId,
                ProgressType = progress.ProgressType, 
                ProgressId = progress.ProgressId, 
                ProgressValue = progress.ProgressValue
            }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<ReadProgressModel>(command);
        }
    }

    public async Task DeleteBookFromRecent(int libraryId, int AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From RecentBooks Where LibraryId = @LibraryId And BookId = @BookId And AccountId = @AccountId;";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, AccountId = AccountId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task AddBookToFavorites(int libraryId, int? AccountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
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
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From FavoriteBooks Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = AccountId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<BookmarkModel>> GetBookmarks(int libraryId, int accountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select * From Bookmarks Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId Order By DateAdded";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<BookmarkModel>(command);
        }
    }

    // Upsert by (BookId, AccountId, ClientId) via MERGE (rather than the two-round-trip
    // check-then-insert AddBookToFavorites uses above) so the OUTPUT clause can return the row -
    // matching AddBook's own "OUTPUT Inserted." idiom - in the same round trip.
    public async Task<BookmarkModel> UpsertBookmark(int libraryId, int accountId, int bookId, string clientId, BookmarkModel bookmark, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Merge Bookmarks As target
                            Using (Select @BookId As BookId, @AccountId As AccountId, @ClientId As ClientId) As source
                            On target.BookId = source.BookId And target.AccountId = source.AccountId And target.ClientId = source.ClientId
                        When Matched Then
                            Update Set ChapterId = @ChapterId, Position = @Position, Name = @Name, DateUpdated = GETDATE()
                        When Not Matched Then
                            Insert (BookId, LibraryId, AccountId, ClientId, ChapterId, Position, Name, DateAdded)
                            Values (@BookId, @LibraryId, @AccountId, @ClientId, @ChapterId, @Position, @Name, GETDATE())
                        Output Inserted.*;";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                AccountId = accountId,
                ClientId = clientId,
                ChapterId = bookmark.ChapterId,
                Position = bookmark.Position,
                Name = bookmark.Name,
            }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookmarkModel>(command);
        }
    }

    public async Task DeleteBookmark(int libraryId, int accountId, int bookId, string clientId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Bookmarks Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId And ClientId = @ClientId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId, ClientId = clientId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<NoteModel>> GetNotes(int libraryId, int accountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select * From Notes Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId Order By DateAdded";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<NoteModel>(command);
        }
    }

    // Same MERGE-based upsert shape as UpsertBookmark above.
    public async Task<NoteModel> UpsertNote(int libraryId, int accountId, int bookId, string clientId, NoteModel note, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Merge Notes As target
                            Using (Select @BookId As BookId, @AccountId As AccountId, @ClientId As ClientId) As source
                            On target.BookId = source.BookId And target.AccountId = source.AccountId And target.ClientId = source.ClientId
                        When Matched Then
                            Update Set ChapterId = @ChapterId, StartOffset = @StartOffset, EndOffset = @EndOffset, Text = @Text, Comment = @Comment, DateUpdated = GETDATE()
                        When Not Matched Then
                            Insert (BookId, LibraryId, AccountId, ClientId, ChapterId, StartOffset, EndOffset, Text, Comment, DateAdded)
                            Values (@BookId, @LibraryId, @AccountId, @ClientId, @ChapterId, @StartOffset, @EndOffset, @Text, @Comment, GETDATE())
                        Output Inserted.*;";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                AccountId = accountId,
                ClientId = clientId,
                ChapterId = note.ChapterId,
                StartOffset = note.StartOffset,
                EndOffset = note.EndOffset,
                Text = note.Text,
                Comment = note.Comment,
            }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<NoteModel>(command);
        }
    }

    public async Task DeleteNote(int libraryId, int accountId, int bookId, string clientId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From Notes Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId And ClientId = @ClientId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId, ClientId = clientId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<RatingModel> GetBookRating(int libraryId, int accountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select * From BookRatings Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<RatingModel>(command);
        }
    }

    // Upsert by (BookId, AccountId) via MERGE so the OUTPUT clause can return the row in the
    // same round trip - same idiom as UpsertBookmark above.
    public async Task<RatingModel> UpsertBookRating(int libraryId, int accountId, int bookId, RatingModel rating, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Merge BookRatings As target
                            Using (Select @BookId As BookId, @AccountId As AccountId) As source
                            On target.BookId = source.BookId And target.AccountId = source.AccountId
                        When Matched Then
                            Update Set Value = @Value, DateUpdated = GETDATE()
                        When Not Matched Then
                            Insert (BookId, LibraryId, AccountId, Value, DateAdded)
                            Values (@BookId, @LibraryId, @AccountId, @Value, GETDATE())
                        Output Inserted.*;";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                BookId = bookId,
                AccountId = accountId,
                Value = rating.Value,
            }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<RatingModel>(command);
        }
    }

    public async Task DeleteBookRating(int libraryId, int accountId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From BookRatings Where LibraryId = @LibraryId And AccountId = @AccountId And BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<RatingSummaryModel> GetBookRatingSummary(int libraryId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Select ISNULL(AVG(CAST(Value AS FLOAT)), 0) As AverageRating, COUNT(*) As TotalCount
                            From BookRatings Where LibraryId = @LibraryId And BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QuerySingleAsync<RatingSummaryModel>(command);
        }
    }

    public async Task DeleteBookContent(int libraryId, int bookId, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete bc
                            From BookContent bc
                            Inner Join Book b On b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            Where b.LibraryId = @LibraryId and b.Id = @BookId And bc.id= @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = contentId, BookId = bookId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName, f.Checksum
                            FROM BookContent bc
                            INNER JOIN Book b ON b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND bc.BookId = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookContentModel>(command);
        }
    }

    public async Task<BookContentModel> GetBookContent(int libraryId, int bookId, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName, f.Checksum
                            FROM BookContent bc
                            INNER JOIN Book b ON b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND bc.BookId = @BookId AND bc.Id = @Id";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, Id = contentId }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<BookContentModel>(command);
        }
    }

    public async Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT bc.Id, bc.BookId, bc.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl, f.FileName As FileName, f.Checksum
                            FROM BookContent bc
                            INNER JOIN Book b ON b.Id = bc.BookId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND bc.BookId = @BookId";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<BookContentModel>(command);
        }
    }

    public async Task UpdateBookContent(int libraryId, int bookId, int contentId, string language, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE bc 
                            SET Language = @Language
                            FROM  BookContent bc 
                            INNER JOIN Book b ON b.Id = bc.BookId
                            Where b.LibraryId = @LibraryId 
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
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO BookContent (BookId, FileId, Language)
                            OUTPUT Inserted.Id VALUES (@BookId, @FileId, @Language)";
            var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId, Language = language }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task UpdateBookImage(int libraryId, int bookId, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update Book
                            Set ImageId = @ImageId
                            Where Id = @BookId And LibraryId = @LibraryId;";
            var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, LibraryId = libraryId }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<PageSummaryModel>> GetBookPageSummary(int libraryId, IEnumerable<int> bookIds, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var bookSummaries = new Dictionary<int, PageSummaryModel>();
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

    private async Task<IEnumerable<BookModel>> GetBooks(IDbConnection connection, int libraryId, List<int> bookIds, CancellationToken cancellationToken, int? AccountId = null)
    {
        var books = new Dictionary<int, BookModel>();
        var sql3 = @"Select b.*, s.Name As SeriesName, fl.FilePath AS ImageUrl,
                            CASE WHEN fb.id IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                            (SELECT COUNT(*) FROM BookPage WHERE BookPage.BookId = b.Id) As PageCount,
                            (SELECT COUNT(*) FROM Chapter WHERE Chapter.BookId = b.Id) As ChapterCount,
                            a.*, c.*, r.*
                            From Book b
                            LEFT JOIN Series s On b.SeriesId = s.id
                            LEFT JOIN FavoriteBooks f On b.Id = f.BookId
                            INNER JOIN BookAuthor ba ON b.Id = ba.BookId
                            INNER JOIN Author a On ba.AuthorId = a.Id
                            LEFT JOIN BookCategory bc ON b.Id = bc.BookId
                            LEFT JOIN Category c ON bc.CategoryId = c.Id
                            LEFT JOIN FavoriteBooks fb On fb.BookId = b.Id
                            LEFT JOIN RecentBooks r On b.Id = r.BookId AND (r.AccountId = @AccountId OR @AccountId Is Null)
                            LEFT OUTER JOIN [File] fl ON fl.Id = b.ImageId
                            Where b.LibraryId = @LibraryId
                            AND b.Id IN @BookList";
        var command3 = new CommandDefinition(sql3, new { LibraryId = libraryId, BookList = bookIds, AccountId = AccountId }, cancellationToken: cancellationToken);

        await connection.QueryAsync<BookModel, AuthorModel, CategoryModel, ReadProgressModel, BookModel>(command3, (b, a, c, r) =>
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

            if (r != null && (r.ProgressType != null || r.ProgressId != 0 || r.DateRead != null))
            {
                book.ReadProgress = r;
            }

            return book;
        }, splitOn: "Id,Id,ProgressType");

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
