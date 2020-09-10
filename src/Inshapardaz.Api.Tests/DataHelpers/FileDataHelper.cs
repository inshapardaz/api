using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class FileDataHelper
    {
        public static void AddFiles(this IDbConnection connection, IEnumerable<FileDto> files) =>
            files.ForEach(f => connection.AddFile(f));

        public static void AddFile(this IDbConnection connection, FileDto file)
        {
            var sql = @"Insert Into [File] (DateCreated, [FileName], MimeType, FilePath, IsPublic)
                        Output Inserted.Id
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic)";
            var id = connection.ExecuteScalar<int>(sql, file);
            file.Id = id;
        }

        public static void DeleteFiles(this IDbConnection connection, IEnumerable<FileDto> file)
        {
            var sql = "Delete From [File] Where Id IN @Ids";
            connection.Execute(sql, new { Ids = file.Select(f => f.Id) });
        }

        public static FileDto GetFileById(this IDbConnection connection, int fileId)
        {
            var sql = "Select * From [File] Where Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = fileId });
        }
    }
}
