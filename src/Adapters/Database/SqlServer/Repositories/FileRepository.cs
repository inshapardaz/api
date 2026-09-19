using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories;

public class FileRepository(SqlServerConnectionProvider connectionProvider) : IFileRepository
{
    public async Task<FileModel> GetFileById(long id, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
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
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Insert Into [File] (FileName, MimeType, FilePath, IsPublic, Checksum, DateCreated)
                            Output Inserted.Id
                            VALUES (@FileName, @MimeType, @FilePath, @IsPublic, @Checksum, GETDATE())"; ;
            var command = new CommandDefinition(sql, file, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetFileById(id, cancellationToken);
    }

    public async Task<FileModel> UpdateFile(FileModel file, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update [File]
                            Set FileName = @FileName,
                                MimeType = @MimeType,
                                FilePath = @FilePath,
                                IsPublic = @IsPublic,
                                Checksum = @Checksum,
                                DateUpdated = GETDATE()
                            Where Id = @Id";
            var command = new CommandDefinition(sql, file, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
            return await GetFileById(file.Id, cancellationToken);
        }
    }

    public async Task DeleteFile(long id, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Delete From [File] Where Id = @Id";
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }
}
