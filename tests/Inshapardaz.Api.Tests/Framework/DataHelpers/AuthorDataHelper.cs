using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class AuthorDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddAuthor(this IDbConnection connection, AuthorDto author)
        {
            var id = _dbType == DatabaseTypes.SqlServer
                ? connection.ExecuteScalar<int>("Insert Into Author (Name, ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @ImageId, @LibraryId)", author)
                : connection.ExecuteScalar<int>("INSERT INTO Author (`Name`, `ImageId`, LibraryId) VALUES (@Name, @ImageId, @LibraryId); SELECT LAST_INSERT_ID();", author); ;
            author.Id = id;
        }

        public static void AddAuthors(this IDbConnection connection, IEnumerable<AuthorDto> authors)
        {
            foreach (var author in authors)
            {
                AddAuthor(connection, author);
            }
        }

        public static void DeleteAuthor(this IDbConnection connection, int authorId)
        {
            var sql = "DELETE FROM Author WHERE Id = @Id";
            connection.Execute(sql, new { Id = authorId });
        }

        public static void DeleteAuthors(this IDbConnection connection, IEnumerable<AuthorDto> authors)
        {
            var sql = "DELETE FROM Author WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = authors.Select(a => a.Id) });
        }

        public static AuthorDto GetAuthorById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<AuthorDto>("SELECT * FROM Author WHERE Id = @Id", new { Id = id });
        }

        public static string GetAuthorImageUrl(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer ? 
                @"SELECT f.FilePath from [File] f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    Where a.Id = @Id"
                : @"SELECT f.FilePath from `File` f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }

        public static FileDto GetAuthorImage(this IDbConnection connection, int id)
        {
            var sql = _dbType == DatabaseTypes.SqlServer ? 
                @"SELECT f.* from [File] f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id"
                : @"SELECT f.* from `File` f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }

        public static IEnumerable<AuthorDto> GetAuthorsByBook(this IDbConnection connection, int bookId)
        {
            var query = @"SELECT * From Author
                    INNER JOIN BookAuthor ba ON Id = ba.AuthorId
                    WHERE ba.BookId = @Id";
            return connection.Query<AuthorDto>(query, new { Id = bookId });
        }

        public static IEnumerable<AuthorDto> GetAuthorsByArticle(this IDbConnection connection, long articleId)
        {
            var query = @"SELECT a.* From ArticleAuthor
                    INNER JOIN Author a ON a.Id = ArticleAuthor.AuthorId
                    WHERE ArticleAuthor.ArticleId = @Id";
            return connection.Query<AuthorDto>(query, new { Id = articleId });
        }
    }
}
