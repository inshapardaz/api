using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Data;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class LibraryDataHelper
    {
        public static void AddLibrary(this IDbConnection connection, LibraryDto library)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Library (Name, Language, SupportsPeriodicals) OUTPUT Inserted.Id VALUES (@Name, @Language, @SupportsPeriodicals)", library);
            library.Id = id;
        }

        public static void DeleteLibrary(this IDbConnection connection, int libraryId)
        {
            connection.Execute("Delete Library Where Id = @LibraryId", new { LibraryId = libraryId });
        }
    }
}
