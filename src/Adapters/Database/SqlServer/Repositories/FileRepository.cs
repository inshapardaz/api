using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly SqlServerConnectionProvider _connectionProvider;

        public FileRepository(SqlServerConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<FileModel> GetFileById(long id, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Select *
                            FROM [File]
                            Where Id = @id";
                var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<FileModel>(command);
            }
        }

        public async Task<FileModel> AddFile(FileModel file, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into [File] (FileName, MimeType, FilePath, IsPublic, DateCreated)
                            Output Inserted.Id
                            VALUES (@FileName, @MimeType, @FilePath, @IsPublic, GETDATE())"; ;
                var command = new CommandDefinition(sql, file, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetFileById(id, cancellationToken);
        }

        public async Task UpdateFile(FileModel file, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update [File]
                            Set FileName = @FileName,
                                MimeType = @MimeType,
                                FilePath = @FilePath,
                                IsPublic = @IsPublic,
                                DateCreated = GETDATE()
                            Where Id = @Id";
                var command = new CommandDefinition(sql, file, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteFile(long id, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Delete From [File] Where Id = @Id";
                var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
