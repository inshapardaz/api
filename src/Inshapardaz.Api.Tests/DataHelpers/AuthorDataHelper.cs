using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class AuthorDataHelper
    {
        public static void AddAuthor(this IDbConnection connection, AuthorDto author)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Author (Name, ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @ImageId, @LibraryId)", author);
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
            var sql = "Delete From Author Where Id = @Id";
            connection.Execute(sql, new { Id = authorId });
        }

        public static void DeleteAuthors(this IDbConnection connection, IEnumerable<AuthorDto> authors)
        {
            var sql = "Delete From Author Where Id IN @Ids";
            connection.Execute(sql, new { Ids = authors.Select(a => a.Id) });
        }

        public static AuthorDto GetAuthorById(this IDbConnection connection, int id)
        {
            return connection.QuerySingleOrDefault<AuthorDto>("Select * From Author Where Id = @Id", new { Id = id });
        }

        public static string GetAuthorImageUrl(this IDbConnection connection, int id)
        {
            var sql = @"Select f.FilePath from [File] f
                        Inner Join Author a ON f.Id = a.ImageId
                        Where a.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }

        public static FileDto GetAuthorImage(this IDbConnection connection, int id)
        {
            var sql = @"Select f.* from [File] f
                        Inner Join Author a ON f.Id = a.ImageId
                        Where a.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }
    }
}
