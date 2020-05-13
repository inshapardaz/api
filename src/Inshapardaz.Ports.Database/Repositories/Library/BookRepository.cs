using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class BookRepository : IBookRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public BookRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<BookModel> AddBook(int libraryId, BookModel book, Guid? userId, CancellationToken cancellationToken)
        {
            int bookId;
            using (var connection = _connectionProvider.GetConnection())
            {
                book.LibraryId = libraryId;
                var sql = @"Insert Into Library.Book
                            (Title, [Description], AuthorId, ImageId, LibraryId, IsPublic, IsPublished, [Language], [Status], SeriesId, SeriesIndex, CopyRights, YearPublished, DateAdded, DateUpdated)
                            OUTPUT Inserted.Id VALUES(@Title, @Description, @AuthorId, @ImageId, @LibraryId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @CopyRights, @YearPublished, GETDATE(), GETDATE());";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                bookId = await connection.ExecuteScalarAsync<int>(command);

                var sqlCategory = @"Delete From Library.BookCategory Where BookId = @BookId;
                           Insert Into Library.BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

                if (book.Categories != null && book.Categories.Any())
                {
                    var bookCategories = book.Categories.Select(c => new { BookId = bookId, CategoryId = c.Id });
                    var commandCategory = new CommandDefinition(sqlCategory, bookCategories, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }

                return await GetBookById(libraryId, bookId, userId, cancellationToken);
            }
        }

        public async Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Book SET
                            Title = @Title, [Description] = @Description,
                            AuthorId = @AuthorId, ImageId = @ImageId,
                            IsPublic = @IsPublic, IsPublished = @IsPublished,
                            [Language] = @Language, [Status] = @Status, SeriesId = @SeriesId,
                            SeriesIndex = @SeriesIndex, CopyRights = @CopyRights,
                            YearPublished = @YearPublished, DateUpdated = GETDATE()
                            Where LibraryId = @LibraryId And Id = @Id";
                var command = new CommandDefinition(sql, book, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                var sqlCategory = @"Delete From Library.BookCategory Where BookId = @BookId;
                           Insert Into Library.BookCategory (BookId, CategoryId) Values (@BookId, @CategoryId);";

                var bookCategories = book.Categories.Select(c => new { BookId = book.Id, CategoryId = c.Id });
                var commandCategory = new CommandDefinition(sqlCategory, bookCategories, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }
        }

        public async Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.Book Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId
                            Order By b.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = "SELECT COUNT(*) FROM Library.Books WHERE LibraryId = @LibraryId";
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

        public async Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Where b.LibraryId = @LibraryId And b.Name Like @Query
                            Order By b.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, Query = $"%{searchText}%", PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = "SELECT COUNT(*) FROM Library.Books Where b.Name Like @Query AND LibraryId = @LibraryId";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, new { LibraryId = libraryId, Query = $"%{searchText}%" }, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<Page<BookModel>> GetLatestBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId
                            Order By b.DateAdded
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = "SELECT COUNT(*) FROM Library.Books WHERE LibraryId = @LibraryId";
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

        public async Task<Page<BookModel>> GetBooksByCategory(int libraryId, int categoryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId And c.Id = @CategoryId
                            Order By b.DateAdded
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, CategoryId = categoryId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = @"Select Count(b.*) From Library.Books b
                                Inner Join Library.BookCategory bc On b.Id = bc.BookId
                                Where LibraryId = @LibraryId And bc.CategoryId = @CategoryId";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, new { LibraryId = libraryId, CategoryId = categoryId }, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<Page<BookModel>> GetBooksByAuthor(int libraryId, int authorId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId And a.Id = @AuthorId
                            Order By b.DateAdded
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, AuthorId = authorId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = @"Select Count(b.*) From Library.Books b
                                Inner Join Library.Author a On b.AuthorId = a.Id
                                Where LibraryId = @LibraryId And a.Id = @AuthorId";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, new { LibraryId = libraryId, AuthorId = authorId }, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<Page<BookModel>> GetBooksBySeries(int libraryId, int seriesId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId And b.SeriesId = @SeriesId
                            Order By b.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, SeriesId = seriesId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = @"Select Count(*) From Library.Books
                                Where LibraryId = @LibraryId And SeriesId = @SeriesId";
                var bookCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, new { LibraryId = libraryId, SeriesId = seriesId }, cancellationToken: cancellationToken));

                return new Page<BookModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = bookCount,
                    Data = books.Values
                };
            }
        }

        public async Task<Page<BookModel>> GetFavoriteBooksByUser(int libraryId, Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId
                            Order By b.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber, UserId = userId },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                var sqlCount = "SELECT COUNT(*) FROM Library.Books WHERE LibraryId = @LibraryId";
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

        public async Task<BookModel> GetBookById(int libraryId, int bookId, Guid? userId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                BookModel book = null;
                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            from Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Left Outer Join Library.Series s On b.SeriesId = s.id
                            Left Outer Join Library.FavoriteBooks f On b.Id = f.BookId AND (f.UserId = @UserId OR @UserId Is Null)
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId AND b.Id = @Id";
                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(sql, (b, c) =>
                {
                    if (book == null)
                    {
                        book = b;
                    }

                    book.Categories.Add(c);
                    return book;
                }, new { LibraryId = libraryId, Id = bookId, UserId = userId == Guid.Empty ? null : userId });

                return book;
            }
        }

        public async Task AddRecentBook(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                // TODO :  Delete to old records
                var sql = @"Delete From Library.RecentBooks Where LibraryId = @LibraryId And BookId = @BookId And UserId = @UserId;
                            Insert Into Library.RecentBooks (BookId, UserId, DateRead, LibraryId) VALUES (@BookId, @UserId, GETDATE(), @LibraryId);";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, UserId = userId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteBookFromRecent(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.RecentBooks Where LibraryId = @LibraryId And BookId = @BookId And UserId = @UserId;";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, BookId = bookId, UserId = userId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<BookModel>> GetRecentBooksByUser(int libraryId, Guid userId, int count, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var books = new Dictionary<int, BookModel>();

                var sql = @"Select b.*, a.Name As AuthorName, s.Name As SeriesName, c.*
                            From Library.Book b
                            Inner Join Library.Author a On b.AuthorId = a.Id
                            Inner Join Library.Series s On b.SeriesId = s.id
                            Inner Join Library.RecentBooks r On b.Id = r.BookId
                            Left Outer Join Library.BookCategory bc ON b.Id = bc.BookId
                            Left Outer Join Library.Category c ON bc.CategoryId = c.Id
                            Where b.LibraryId = @LibraryId And r.UserId = @UserId
                            Order By r.DateRead
                            OFFSET 0 ROWS
                            FETCH NEXT @Count ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, UserId = userId, Count = count },
                                                    cancellationToken: cancellationToken);

                await connection.QueryAsync<BookModel, CategoryModel, BookModel>(command, (b, c) =>
                {
                    if (!books.TryGetValue(b.Id, out BookModel book))
                        books.Add(b.Id, book = b);

                    book.Categories.Add(c);
                    return book;
                });

                return books.Values;
            }
        }

        public async Task AddBookToFavorites(int libraryId, Guid? userId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var check = "Select count(*) From Library.FavoriteBooks Where LibraryId = @LibraryId And UserId = @UserId And BookId = @BookId;";
                var commandCheck = new CommandDefinition(check, new { LibraryId = libraryId, UserId = userId, Id = bookId }, cancellationToken: cancellationToken);
                var count = await connection.ExecuteScalarAsync<int>(commandCheck);

                if (count > 0) return;

                var sql = @"Delete From Library.FavoriteBooks Where LibraryId = @LibraryId And UserId = @UserId And BookId = @BookId;";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, UserId = userId, Id = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteBookFromFavorites(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.FavoriteBooks Where LibraryId = @LibraryId And UserId = @UserId And BookId = @BookId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, UserId = userId, Id = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteBookFile(int bookId, int fileId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.BookFile Where BookId = @BookId And FileId = @FileId";
                var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<FileModel> GetBookFileById(int bookId, int fileId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from Library.BookFile bf
                            Inner Join Inshapardaz.[File] f On bf.FileId = f.Id
                            Where bf.BookId = @BookId And bf.FileId = @FileId;";
                var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<FileModel>(command);
            }
        }

        public async Task<IEnumerable<FileModel>> GetFilesByBook(int bookId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from Library.BookFile bf
                            Inner Join Inshapardaz.[File] f On bf.FileId = f.Id
                            Where bf.BookId = @BookId";
                var command = new CommandDefinition(sql, new { BookId = bookId }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<FileModel>(command);
            }
        }

        public async Task AddBookFile(int bookId, int fileId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Library.BookFile (BookId, FileId) Values (@BookId, @FileId)";
                var command = new CommandDefinition(sql, new { FileId = fileId, BookId = bookId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task UpdateBookImage(int libraryId, int bookId, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Book
                            Set ImageId = @ImageId
                            Where Id = @BookId And LibraryId = @LibraryId;";
                var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, LibraryId = libraryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
