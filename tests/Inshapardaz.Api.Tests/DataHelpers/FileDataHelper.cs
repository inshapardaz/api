using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class FileDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddFiles(this IDbConnection connection, IEnumerable<FileDto> files) =>
            files.ForEach(f => connection.AddFile(f));

        public static void AddFile(this IDbConnection connection, FileDto file)
        {
            var sql = @"Insert Into [File] (DateCreated, [FileName], MimeType, FilePath, IsPublic)
                        Output Inserted.Id
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic)";
            var mySql = @"INSERT INTO `File` (DateCreated, `FileName`, MimeType, FilePath, IsPublic)
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic);
                        SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, file);
            file.Id = id;
        }

        public static void DeleteFiles(this IDbConnection connection, IEnumerable<FileDto> files)
        {
            var sql = "Delete From [File] Where Id IN @Ids";
            var mySql = "Delete From `File` Where Id IN @Ids";
            connection.Execute(_dbType == DatabaseTypes.SqlServer ? sql : mySql, new { Ids = files.Select(f => f.Id) });
        }

        public static FileDto GetFileById(this IDbConnection connection, long fileId)
        {
            var sql = "Select * From [File] Where Id = @Id";
            var mySql = "Select * From `File` Where Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, new { Id = fileId });
        }
    }
}
