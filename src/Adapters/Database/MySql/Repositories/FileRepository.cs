using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.MySql.Repositories;

public class FileRepository(MySqlConnectionProvider connectionProvider) : IFileRepository
{
    public async Task<FileModel> GetFileById(long id, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT *
                            FROM `File`
                            WHERE Id = @id";
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<FileModel>(command);
        }
    }

    public async Task<FileModel> AddFile(FileModel file, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO `File` (FileName, MimeType, FilePath, IsPublic, Checksum, DateCreated)
                            VALUES (@FileName, @MimeType, @FilePath, @IsPublic, @Checksum, UTC_TIMESTAMP());
                            SELECT LAST_INSERT_ID()";
            var command = new CommandDefinition(sql, file, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetFileById(id, cancellationToken);
    }

    public async Task<FileModel> UpdateFile(FileModel file, CancellationToken cancellationToken)
    {
        using (var connection = connectionProvider.GetLibraryConnection())
        {
            var sql = @"Update `File`
                            Set FileName = @FileName,
                                MimeType = @MimeType,
                                FilePath = @FilePath,
                                IsPublic = @IsPublic,
                                Checksum = @Checksum,
                                DateUpdated = UTC_TIMESTAMP()
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
            var sql = @"DELETE FROM `File` WHERE Id = @Id";
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }
}
