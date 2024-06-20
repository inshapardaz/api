using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IFileTestRepository
    {
        void AddFiles(IEnumerable<FileDto> files);
        void AddFile(FileDto file);
        void DeleteFiles(IEnumerable<FileDto> files);
        FileDto GetFileById(long fileId);

    }

    public class MySqlFileTestRepository : IFileTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlFileTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddFiles(IEnumerable<FileDto> files) =>
            files.ForEach(f => AddFile(f));

        public void AddFile(FileDto file)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var mySql = @"INSERT INTO `File` (DateCreated, `FileName`, MimeType, FilePath, IsPublic)
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic);
                        SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(mySql, file);
                file.Id = id;
            }
        }

        public void DeleteFiles(IEnumerable<FileDto> files)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var mySql = "Delete From `File` Where Id IN @Ids";
                connection.Execute(mySql, new { Ids = files.Select(f => f.Id) });
            }
        }

        public FileDto GetFileById(long fileId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var mySql = "Select * From `File` Where Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(mySql, new { Id = fileId });
            }
        }
    }
    public class SqlServerFileTestRepository : IFileTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerFileTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddFiles(IEnumerable<FileDto> files) =>
            files.ForEach(f => AddFile(f));

        public void AddFile(FileDto file)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into [File] (DateCreated, [FileName], MimeType, FilePath, IsPublic)
                        Output Inserted.Id
                        Values (@DateCreated, @FileName, @MimeType, @FilePath, @IsPublic)";
                var id = connection.ExecuteScalar<int>(sql, file);
                file.Id = id;
            }
        }

        public void DeleteFiles(IEnumerable<FileDto> files)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Delete From [File] Where Id IN @Ids";
                connection.Execute(sql, new { Ids = files.Select(f => f.Id) });
            }
        }

        public FileDto GetFileById(long fileId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Select * From [File] Where Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = fileId });
            }
        }
    }

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
