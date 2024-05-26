using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class LibraryDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddLibraries(this IDbConnection connection, IEnumerable<LibraryDto> libraries)
        {
            foreach (var library in libraries)
            {
                AddLibrary(connection, library);
            }
        }

        public static void AddLibrary(this IDbConnection connection, LibraryDto library)
        {

            var sql = @"INSERT INTO Library (Name, Description, Language, SupportsPeriodicals, PrimaryColor, SecondaryColor, ImageId)
                        OUTPUT Inserted.Id 
                        VALUES (@Name, @Description, @Language, @SupportsPeriodicals, @PrimaryColor, @SecondaryColor, @ImageId)";
            var mySql = @"INSERT INTO Library (Name, Description, Language, SupportsPeriodicals, PrimaryColor, SecondaryColor, ImageId) 
                        VALUES (@Name, @Description, @Language, @SupportsPeriodicals, @PrimaryColor, @SecondaryColor, @ImageId);
                        SELECT LAST_INSERT_ID();";
            
            var id = connection.ExecuteScalar<int>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, library);
            library.Id = id;
        }

        public static LibraryDto GetLibrary(this IDbConnection connection, LibraryView library)
        {
            var sql = "Select * From Library Where Name = @Name AND Language = @Language AND SupportsPeriodicals = @SupportsPeriodicals";
            return connection.QuerySingleOrDefault<LibraryDto>(sql,
                new { Name = library.Name, Language = library.Language, SupportsPeriodicals = library.SupportsPeriodicals });
        }

        public static LibraryDto GetLibraryById(this IDbConnection connection, int libraryId)
        {
            return connection.QuerySingleOrDefault<LibraryDto>("Select * From Library Where Id = @Id",
                new { Id = libraryId });
        }

        public static void DeleteLibraries(this IDbConnection connection, IEnumerable<int> libraryIds)
        {
            connection.Execute("DELETE FROM Library WHERE Id IN @LibraryIds", new { LibraryIds = libraryIds });
        }

        public static void DeleteLibrary(this IDbConnection connection, int libraryId)
        {
            connection.Execute("DELETE FROM Library WHERE Id = @LibraryId", new { LibraryId = libraryId });
            connection.Execute("DELETE FROM AccountLibrary WHERE LibraryId = @LibraryId", new { LibraryId = libraryId });
        }

        public static void AssignLibrariesToUser(this IDbConnection connection, IEnumerable<LibraryDto> libraries, int accountId, Role role)
        {
            connection.Execute("INSERT INTO AccountLibrary (LibraryId, AccountId, Role) VALUES (@LibraryId, @AccountId, @Role)",
            libraries.Select(l => new { LibraryId = l.Id, AccountId = accountId, Role = role }));
        }

        public static FileDto GetLibraryImage(this IDbConnection connection, int id)
        {
            var sql = @"SELECT f.* FROM [File] f
                        INNER JOIN Library l ON f.Id = l.ImageId
                        WHERE l.Id = @Id";
            var mySql = @"SELECT f.* FROM `File` f
                        INNER JOIN Library l ON f.Id = l.ImageId
                        WHERE l.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, new { Id = id });
        }

        public static string GetLibraryImageUrl(this IDbConnection connection, int id)
        {
            var sql = @"SELECT f.FilePath FROM [File] f
                        INNER JOIN Library l ON f.Id = l.ImageId
                        WHERE l.Id = @Id";
            var mySql = @"SELECT f.FilePath FROM `File` f
                        INNER JOIN Library l ON f.Id = l.ImageId
                        WHERE l.Id = @Id";
            return connection.QuerySingleOrDefault<string>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, new { Id = id });
        }
    }
}
