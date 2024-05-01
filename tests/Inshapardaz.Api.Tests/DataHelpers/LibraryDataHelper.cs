using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class LibraryDataHelper
    {
        public static void AddLibraries(this IDbConnection connection, IEnumerable<LibraryDto> libraries)
        {
            foreach (var library in libraries)
            {
                AddLibrary(connection, library);
            }
        }

        public static void AddLibrary(this IDbConnection connection, LibraryDto library)
        {
            var id = connection.ExecuteScalar<int>(@"INSERT INTO Library (Name, Description, Language, SupportsPeriodicals, PrimaryColor, SecondaryColor, ImageId)
                        OUTPUT Inserted.Id 
                        VALUES (@Name, @Description, @Language, @SupportsPeriodicals, @PrimaryColor, @SecondaryColor, @ImageId)", library);
            library.Id = id;
        }

        public static LibraryDto GetLibrary(this IDbConnection connection, LibraryView library)
        {
            return connection.QuerySingleOrDefault<LibraryDto>("Select * From Library Where Name = @Name AND Language = @Language AND SupportsPeriodicals = @SupportsPeriodicals",
                new { Name = library.Name, Language = library.Language, SupportsPeriodicals = library.SupportsPeriodicals });
        }

        public static LibraryDto GetLibraryById(this IDbConnection connection, int libraryId)
        {
            return connection.QuerySingleOrDefault<LibraryDto>("Select * From Library Where Id = @Id",
                new { Id = libraryId });
        }

        public static void DeleteLibraries(this IDbConnection connection, IEnumerable<int> libraryIds)
        {
            connection.Execute("Delete Library Where Id IN @LibraryIds", new { LibraryIds = libraryIds });
        }

        public static void DeleteLibrary(this IDbConnection connection, int libraryId)
        {
            connection.Execute("Delete Library Where Id = @LibraryId", new { LibraryId = libraryId });
            connection.Execute("Delete AccountLibrary Where LibraryId = @LibraryId", new { LibraryId = libraryId });
        }

        public static void AssignLibrariesToUser(this IDbConnection connection, IEnumerable<LibraryDto> libraries, int accountId, Role role)
        {
            connection.Execute("INSERT INTO AccountLibrary (LibraryId, AccountId, Role) VALUES (@LibraryId, @AccountId, @Role)",
            libraries.Select(l => new { LibraryId = l.Id, AccountId = accountId, Role = role }));
        }

        public static FileDto GetLibraryImage(this IDbConnection connection, int id)
        {
            var sql = @"Select f.* from [File] f
                        Inner Join Library l ON f.Id = l.ImageId
                        Where l.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
        }

        public static string GetLibraryImageUrl(this IDbConnection connection, int id)
        {
            var sql = @"Select f.FilePath from [File] f
                        Inner Join Library l ON f.Id = l.ImageId
                        Where l.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
        }
    }
}
