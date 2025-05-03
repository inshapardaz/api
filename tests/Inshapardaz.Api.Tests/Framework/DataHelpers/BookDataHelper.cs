using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IBookTestRepository
    {
        void AddBook(BookDto book);
        void AddBooks(IEnumerable<BookDto> books);
        void AddBookToFavorites(int libraryId, int bookId, int accountId);

        void AddBooksToFavorites(int libraryId, IEnumerable<int> bookIds, int accountId);
        void AddBooksToRecentReads(int libraryId, IEnumerable<int> bookIds, int accountId);

        void AddBookToRecentReads(int libraryId, int bookId, int accountId, DateTime? timestamp = null);

        void AddBookToRecentReads(RecentBookDto dto);

        void AddBookFiles(int bookId, IEnumerable<BookContentDto> contentDto);
        void AddBookFile(int bookId, BookContentDto contentDto);

        int GetBookCountByAuthor(int id);

        BookDto GetBookById(int bookId);

        IEnumerable<BookDto> GetBooksByAuthor(int id);

        IEnumerable<BookDto> GetBooksByCategory(int categoryId);

        IEnumerable<BookDto> GetBooksBySeries(int seriesId);

        string GetBookImageUrl(int bookId);

        FileDto GetBookImage(int bookId);

        void DeleteBooks(IEnumerable<BookDto> books);

        bool DoesBookExistsInFavorites(int bookId, int accountId);

        //TODO : Add user id.
        bool DoesBookExistsInRecent(int bookId);
        IEnumerable<BookContentDto> GetBookContents(int bookId);
        BookContentDto GetBookContent(int bookId, string language, string mimetype);

        BookContentDto GetBookContent(int bookId, long contentId);


        string GetBookContentPath(int bookId, string language, string mimetype);



        void AddBookAuthor(int bookId, int authorId);


        void AddBooksAuthor(IEnumerable<int> bookIds, int authorId);
        ReadProgressDto GetBookProgress(int bookId, int accountId);

    }

    public class MySqlBookTestRepository : IBookTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlBookTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddBook(BookDto book)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Book (`Title`, `Description`, ImageId, IsPublic, IsPublished, `Language`, `Status`, SeriesId, SeriesIndex, Copyrights, YearPublished, DateAdded, DateUpdated, LibraryId)
                        VALUES (@Title, @Description, @ImageId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @Copyrights, @YearPublished, @DateAdded, @DateUpdated, @LibraryId);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, book);
                book.Id = id;
            }
        }

        public void AddBooks(IEnumerable<BookDto> books)
        {
            foreach (var book in books)
            {
                AddBook(book);
            }
        }

        public void AddBookToFavorites(int libraryId, int bookId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO FavoriteBooks (LibraryId, BookId, AccountId, DateAdded)
                        VALUES (@LibraryId, @BookId, @AccountId, UTC_TIMESTAMP())";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId });
            }
        }

        public void AddBooksToFavorites(int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => AddBookToFavorites(libraryId, id, accountId));
        }

        public void AddBooksToRecentReads(int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => AddBookToRecentReads(libraryId, id, accountId));
        }

        public void AddBookToRecentReads(int libraryId, int bookId, int accountId, DateTime? timestamp = null)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        VALUES (@LibraryId, @BookId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId, DateRead = timestamp ?? DateTime.Now });
            }
        }

        public void AddBookToRecentReads(RecentBookDto dto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        VALUES (@LibraryId, @BookId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, dto);
            }
        }

        public void AddBookFiles(int bookId, IEnumerable<BookContentDto> contentDto) =>
            contentDto.ForEach(f => AddBookFile(bookId, f));

        public void AddBookFile(int bookId, BookContentDto contentDto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO BookContent (BookId, FileId, Language)
                        VALUES (@BookId, @FileId, @Language);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, new { BookId = bookId, FileId = contentDto.FileId, Language = contentDto.Language });
                contentDto.Id = id;
            }
        }

        public int GetBookCountByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT COUNT(*)
                        FROM Book b
                        INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                        WHERE ba.AuthorId = @Id";
                return connection.ExecuteScalar<int>(sql, new { Id = id });
            }
        }

        public BookDto GetBookById(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Book WHERE Id = @Id";
                return connection.QuerySingleOrDefault<BookDto>(sql, new { Id = bookId });
            }
        }

        public IEnumerable<BookDto> GetBooksByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.*
                        FROM Book b
                        LEFT OUTER JOIN BookAuthor ba ON b.Id = ba.BookId
                        INNER JOIN Author a On ba.AuthorId = a.Id
                        WHERE a.AuthorId = @Id";
                return connection.Query<BookDto>(sql, new { Id = id });
            }
        }

        public IEnumerable<BookDto> GetBooksByCategory(int categoryId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.* FROM Book b
                        INNER JOIN BookCategory bc ON b.Id = bc.BookId
                        WHERE bc.CategoryId = @CategoryId";
                return connection.Query<BookDto>(sql, new { CategoryId = categoryId });
            }
        }

        public IEnumerable<BookDto> GetBooksBySeries(int seriesId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.* FROM Book b WHERE SeriesId = @SeriesId";
                return connection.Query<BookDto>(sql, new { SeriesId = seriesId });
            }
        }

        public string GetBookImageUrl(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM `File` f
                        INNER JOIN Book b ON f.Id = b.ImageId
                        WHERE b.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = bookId });
            }
        }

        public FileDto GetBookImage(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from `File` f
                        Inner Join Book b ON f.Id = b.ImageId
                        Where b.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = bookId });
            }
        }

        public void DeleteBooks(IEnumerable<BookDto> books)
        {
            if (books != null && books.Any())
            {
                using (var connection = _connectionProvider.GetConnection())
                {
                    var sql = "DELETE FROM Book WHERE Id IN @Ids";
                    connection.Execute(sql, new { Ids = books.Select(f => f.Id) });
                }
            }
        }

        public bool DoesBookExistsInFavorites(int bookId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM FavoriteBooks WHERE BookId = @BookId AND AccountId = @AccountId", new
                {
                    BookId = bookId,
                    AccountId = accountId
                });
            }
        }

        //TODO : Add user id.
        public bool DoesBookExistsInRecent(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM RecentBooks WHERE BookId = @BookId", new
                {
                    BookId = bookId
                });
            }
        }

        public IEnumerable<BookContentDto> GetBookContents(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT bc.*, f.MimeType FROM BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join `File` f ON f.Id = bc.FileId
                           Where b.Id = @BookId";

                return connection.Query<BookContentDto>(sql, new
                {
                    BookId = bookId
                });
            }
        }

        public BookContentDto GetBookContent(int bookId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join `File` f ON f.Id = bc.FileId
                           WHERE b.Id = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<BookContentDto>(sql, new
                {
                    BookId = bookId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public BookContentDto GetBookContent(int bookId, long contentId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join `File` f ON f.Id = bc.FileId
                           WHERE b.Id = @BookId AND bc.Id = @ContentId";

                return connection.QuerySingleOrDefault<BookContentDto>(sql, new
                {
                    BookId = bookId,
                    ContentId = contentId
                });
            }
        }

        public string GetBookContentPath(int bookId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT f.FilePath FROM BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join `File` f ON f.Id = bc.FileId
                           WHERE b.BookId = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<string>(sql, new
                {
                    BookId = bookId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public void AddBookAuthor(int bookId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO BookAuthor (BookId, AuthorId) VALUES (@BookId, @AuthorId)";
                connection.Execute(sql, new { BookId = bookId, AuthorId = authorId });
            }
        }

        public void AddBooksAuthor(IEnumerable<int> bookIds, int authorId)
        {
            foreach (var bookId in bookIds)
            {
                AddBookAuthor(bookId, authorId);
            }
        }

        public ReadProgressDto GetBookProgress(int bookId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "SELECT * FROM RecentBooks WHERE BookId = @BookId AND AccountId = @AccountId";
                return connection.QueryFirstOrDefault<ReadProgressDto>(sql, new { BookId = bookId, AccountId = accountId });
            }
        }
    }

    public class SqlServerBookTestRepository : IBookTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerBookTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddBook(BookDto book)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Book (Title, Description, ImageId, IsPublic, IsPublished, Language, Status, SeriesId, SeriesIndex, Copyrights, YearPublished, DateAdded, DateUpdated, LibraryId)
                        OUTPUT Inserted.Id
                        VALUES (@Title, @Description, @ImageId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @Copyrights, @YearPublished, @DateAdded, @DateUpdated, @LibraryId)";
                var id = connection.ExecuteScalar<int>(sql, book);
                book.Id = id;
            }
        }

        public void AddBooks(IEnumerable<BookDto> books)
        {
            foreach (var book in books)
            {
                AddBook(book);
            }
        }

        public void AddBookToFavorites(int libraryId, int bookId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO FavoriteBooks (LibraryId, BookId, AccountId, DateAdded)
                        VALUES (@LibraryId, @BookId, @AccountId, UTC_TIMESTAMP())";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId });
            }
        }

        public void AddBooksToFavorites(int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => AddBookToFavorites(libraryId, id, accountId));
        }

        public void AddBooksToRecentReads(int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => AddBookToRecentReads(libraryId, id, accountId));
        }

        public void AddBookToRecentReads(int libraryId, int bookId, int accountId, DateTime? timestamp = null)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        VALUES (@LibraryId, @BookId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId, DateRead = timestamp ?? DateTime.Now });
            }
        }

        public void AddBookToRecentReads(RecentBookDto dto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        VALUES (@LibraryId, @BookId, @AccountId, @DateRead)";
                connection.ExecuteScalar<int>(sql, dto);
            }
        }

        public void AddBookFiles(int bookId, IEnumerable<BookContentDto> contentDto) =>
            contentDto.ForEach(f => AddBookFile(bookId, f));

        public void AddBookFile(int bookId, BookContentDto contentDto)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO BookContent (BookId, FileId, Language)
                        OUTPUT Inserted.Id
                        VALUES (@BookId, @FileId, @Language)";
                var id = connection.ExecuteScalar<int>(sql, new { BookId = bookId, FileId = contentDto.FileId, Language = contentDto.Language });
                contentDto.Id = id;
            }
        }

        public int GetBookCountByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT COUNT(*)
                        FROM Book b
                        INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                        WHERE ba.AuthorId = @Id";
                return connection.ExecuteScalar<int>(sql, new { Id = id });
            }
        }

        public BookDto GetBookById(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Book WHERE Id = @Id";
                return connection.QuerySingleOrDefault<BookDto>(sql, new { Id = bookId });
            }
        }

        public IEnumerable<BookDto> GetBooksByAuthor(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.*
                        FROM Book b
                        LEFT OUTER JOIN BookAuthor ba ON b.Id = ba.BookId
                        INNER JOIN Author a On ba.AuthorId = a.Id
                        WHERE a.AuthorId = @Id";
                return connection.Query<BookDto>(sql, new { Id = id });
            }
        }

        public IEnumerable<BookDto> GetBooksByCategory(int categoryId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.* FROM Book b
                        INNER JOIN BookCategory bc ON b.Id = bc.BookId
                        WHERE bc.CategoryId = @CategoryId";
                return connection.Query<BookDto>(sql, new { CategoryId = categoryId });
            }
        }

        public IEnumerable<BookDto> GetBooksBySeries(int seriesId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT b.* FROM Book b WHERE SeriesId = @SeriesId";
                return connection.Query<BookDto>(sql, new { SeriesId = seriesId });
            }
        }

        public string GetBookImageUrl(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath FROM [File] f
                        INNER JOIN Book b ON f.Id = b.ImageId
                        WHERE b.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = bookId });
            }
        }

        public FileDto GetBookImage(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select f.* from [File] f
                        Inner Join Book b ON f.Id = b.ImageId
                        Where b.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = bookId });
            }
        }

        public void DeleteBooks(IEnumerable<BookDto> books)
        {
            if (books != null && books.Any())
            {
                using (var connection = _connectionProvider.GetConnection())
                {
                    var sql = "DELETE FROM Book WHERE Id IN @Ids";
                    connection.Execute(sql, new { Ids = books.Select(f => f.Id) });
                }
            }
        }

        public bool DoesBookExistsInFavorites(int bookId, int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM FavoriteBooks WHERE BookId = @BookId AND AccountId = @AccountId", new
                {
                    BookId = bookId,
                    AccountId = accountId
                });
            }
        }

        //TODO : Add user id.
        public bool DoesBookExistsInRecent(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingle<bool>(@"SELECT COUNT(1) FROM RecentBooks WHERE BookId = @BookId", new
                {
                    BookId = bookId
                });
            }
        }

        public IEnumerable<BookContentDto> GetBookContents(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT bc.*, f.MimeType FROM BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join [File] f ON f.Id = bc.FileId
                           Where b.Id = @BookId";

                return connection.Query<BookContentDto>(sql, new
                {
                    BookId = bookId
                });
            }
        }

        public BookContentDto GetBookContent(int bookId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join [File] f ON f.Id = bc.FileId
                           WHERE b.Id = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<BookContentDto>(sql, new
                {
                    BookId = bookId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public BookContentDto GetBookContent(int bookId, long contentId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"Select * From BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join [File] f ON f.Id = bc.FileId
                           WHERE b.Id = @BookId AND bc.Id = @ContentId";

                return connection.QuerySingleOrDefault<BookContentDto>(sql, new
                {
                    BookId = bookId,
                    ContentId = contentId
                });
            }
        }

        public string GetBookContentPath(int bookId, string language, string mimetype)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                string sql = @"SELECT f.FilePath FROM BookContent bc
                           INNER Join Book b ON b.Id = bc.BookId
                           INNER Join [File] f ON f.Id = bc.FileId
                           WHERE b.BookId = @BookId AND bc.Language = @Language AND f.MimeType = @MimeType";

                return connection.QuerySingleOrDefault<string>(sql, new
                {
                    BookId = bookId,
                    Language = language,
                    MimeType = mimetype
                });
            }
        }

        public void AddBookAuthor(int bookId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO BookAuthor (BookId, AuthorId) VALUES (@BookId, @AuthorId)";
                connection.Execute(sql, new { BookId = bookId, AuthorId = authorId });
            }
        }

        public void AddBooksAuthor(IEnumerable<int> bookIds, int authorId)
        {
            foreach (var bookId in bookIds)
            {
                AddBookAuthor(bookId, authorId);
            }
        }

        public ReadProgressDto GetBookProgress(int bookId, int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
