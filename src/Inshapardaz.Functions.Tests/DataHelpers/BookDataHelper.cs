using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class BookDataHelper
    {
        public static void AddBook(this IDbConnection connection, BookDto book)
        {
            var sql = @"Insert Into Library.Book (Title, Description, AuthorId, ImageId, IsPublic, IsPublished, Language, Status, SeriesId, SeriesIndex, Copyrights, YearPublished, DateAdded, DateUpdated, LibraryId)
                        Output Inserted.Id
                        Values (@Title, @Description, @AuthorId, @ImageId, @IsPublic, @IsPublished, @Language, @Status, @SeriesId, @SeriesIndex, @Copyrights, @YearPublished, @DateAdded, @DateUpdated, @LibraryId)";
            var id = connection.ExecuteScalar<int>(sql, book);
            book.Id = id;
        }

        public static void AddBooks(this IDbConnection connection, IEnumerable<BookDto> books)
        {
            foreach (var book in books)
            {
                AddBook(connection, book);
            }
        }

        public static int GetBookCountByAuthor(this IDbConnection connection, int id)
        {
            return connection.ExecuteScalar<int>("Select Count(*) From Library.Book Where AuthorId = @Id", new { Id = id });
        }

        public static IEnumerable<BookDto> GetBooksByAuthor(this IDbConnection connection, int id)
        {
            return connection.Query<BookDto>("Select * From Library.Book Where AuthorId = @Id", new { Id = id });
        }

        public static IEnumerable<BookDto> GetBooksByCategory(this IDbConnection connection, int categoryId)
        {
            return connection.Query<BookDto>(@"Select b.* From Library.Book b
                Inner Join Library.BookCategory bc ON b.Id = bc.BookId
                Where bc.CategoryId = @CategoryId", new { CategoryId = categoryId });
        }

        public static IEnumerable<BookDto> GetBooksBySeries(this IDbConnection connection, int seriesId)
        {
            return connection.Query<BookDto>(@"Select * From Library.Book Where SeriesId = @SeriesId ", new { SeriesId = seriesId });
        }
    }
}
