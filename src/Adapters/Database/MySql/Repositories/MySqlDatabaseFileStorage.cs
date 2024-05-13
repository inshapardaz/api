using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories
{
    public class MySqlDatabaseFileStorage : IFileStorage
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public bool SupportsPublicLink => false;

        public MySqlDatabaseFileStorage(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Delete From [FileData] Where Path = @Path";
                var command = new CommandDefinition(sql, new { Path = filePath }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            return DeleteFile(filePath, cancellationToken);
        }

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Select [content]
                            From [FileData]
                            Where Path = @Path";
                var command = new CommandDefinition(sql, new { Path = filePath }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<byte[]>(command);
            }
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            var content = await GetFile(filePath, cancellationToken);
            return System.Text.Encoding.Default.GetString(content);
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var path = $"{Guid.NewGuid():N}/{name}";
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into [FileData] (Path, Content)
                            VALUES (@Path, @Content)"; ;
                var command = new CommandDefinition(sql, new { Path = path, Content = content }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }

            return path;
        }

        public Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            return StoreFile(name, content, cancellationToken);
        }

        public Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            return StoreFile(name, System.Text.Encoding.Default.GetBytes(content), cancellationToken);
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                await DeleteFile(filePath, cancellationToken);
            }
            finally
            {
            }
        }

        public Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            return DeleteFile(filePath, cancellationToken);
        }

        public string GetPublicUrl(string filePath)
        {
            return filePath;
        }
    }
}
