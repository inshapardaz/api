using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class AuthorDataHelper
    {
        public static void AddAuthor(this IDbConnection connection, AuthorDto author)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Library.Author (Name, ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @ImageId, @LibraryId)", author);
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
            var sql = "Delete From Library.Author Where Id = @Id";
            connection.Execute(sql, new { Id = authorId });
        }

        public static void DeleteAuthors(this IDbConnection connection, IEnumerable<AuthorDto> authors)
        {
            var sql = "Delete From Library.Author Where Id IN @Ids";
            connection.Execute(sql, new { Ids = authors.Select(a => a.Id) });
        }

        public static AuthorDto GetAuthorById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<AuthorDto>("Select * From Library.Author Where Id = @Id", new { Id = id });
        }

        public static string GetAuthorImageUrl(this IDbConnection connection, int id)
        {
            var sql = @"Select f.FilePath from Inshapardaz.[File] f
                        Inner Join Library.Author a ON f.Id = a.ImageId
                        Where a.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }
    }
}
