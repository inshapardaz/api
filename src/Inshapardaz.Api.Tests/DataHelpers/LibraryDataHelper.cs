using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views;
using System.Data;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class LibraryDataHelper
    {
        public static void AddLibrary(this IDbConnection connection, LibraryDto library)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Library (Name, Language, SupportsPeriodicals) OUTPUT Inserted.Id VALUES (@Name, @Language, @SupportsPeriodicals)", library);
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

        public static void DeleteLibrary(this IDbConnection connection, int libraryId)
        {
            connection.Execute("Delete Library Where Id = @LibraryId", new { LibraryId = libraryId });
        }
    }
}
