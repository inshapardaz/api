using Dapper;
using Inshapardaz.Functions.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.DataHelpers
{
    public static class FileDataHelper
    {
        public static void AddFile(this IDbConnection connection, FileDto file)
        {
            var sql = @"Insert Into Inshapardaz.[File] (DateCreated, [FileName], MimeType, FilePath, IsPublic)
                        Output Inserted.Id
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic)";
            var id = connection.ExecuteScalar<int>(sql, file);
            file.Id = id;
        }

        public static void DeleteFiles(this IDbConnection connection, IEnumerable<FileDto> file)
        {
            var sql = "Delete From Inshapardaz.[File] Where Id IN @Ids";
            connection.Execute(sql, new { Ids = file.Select(f => f.Id) });
        }
    }
}
