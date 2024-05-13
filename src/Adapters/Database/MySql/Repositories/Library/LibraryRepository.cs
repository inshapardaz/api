using Dapper;
using DocumentFormat.OpenXml.Drawing;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public LibraryRepository(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<LibraryModel>> GetLibraries(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library
                                LEFT OUTER JOIN `File` f ON f.Id = ImageId
                            ORDER BY `Name`
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { PageSize = pageSize, Offset = pageSize * (pageNumber -1)},
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> FindLibraries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library
                                LEFT OUTER JOIN `File` f ON f.Id = ImageId
                            WHERE `Name` LIKE @Query
                            ORDER BY `Name`
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", PageSize = pageSize, Offset = pageSize * (pageNumber -1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library WHERE `Name` LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> GetUserLibraries(int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                                INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId
                            ORDER BY l.Name
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { AccountId = accountId, PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                                       FROM Library l
                                        INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                                       WHERE al.AccountId = @AccountId";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount,
                    new { AccountId = accountId },
                    cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> FindUserLibraries(string query, int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                                INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId AND Name LIKE @Query
                            ORDER BY l.Name
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", AccountId = accountId, PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                            FROM Library l
                                INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId 
                                AND Name LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%", AccountId = accountId }, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> GetUnassignedLibraries(int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                                LEFT JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId != @AccountId
                            ORDER BY l.Name
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { AccountId = accountId, PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                                       FROM Library l
                                        LEFT JOIN AccountLibrary al ON al.LibraryId = l.Id
                                       WHERE al.AccountId != @AccountId";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount,
                    new { AccountId = accountId },
                    cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> FindUnassignedLibraries(string query, int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                                LEFT JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId != @AccountId AND Name LIKE @Query
                            ORDER BY l.Name
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", AccountId = accountId, PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                            FROM Library l
                                LEFT JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId != @AccountId 
                                AND Name LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%", AccountId = accountId }, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> GetPublicLibraries(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library
                                LEFT OUTER JOIN `File` f ON f.Id = ImageId
                            WHERE [Public] = 1
                            ORDER BY Name
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> FindPublicLibraries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library
                                LEFT OUTER JOIN `File` f ON f.Id = ImageId
                            WHERE `Name` LIKE @Query AND [Public] = 1
                            ORDER BY `Name`
                            LIMIT @PageSize
                            OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", PageSize = pageSize, Offset = pageSize * (pageNumber - 1) },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library WHERE `Name` LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }
        public async Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken)
        {
            int libraryId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Library(`Name`, `Language`, SupportsPeriodicals, PrimaryColor, SecondaryColor, OwnerEmail, `Public`, DatabaseConnection, FileStoreType, FileStoreSource) 
                            VALUES(@Name, @Language, @SupportsPeriodicals, @PrimaryColor, @SecondaryColor, @OwnerEmail, @Public, @DatabaseConnection, @FileStoreType, @FileStoreSource);
                            SELECT LAST_INSERT_ID();";
                var command = new CommandDefinition(sql, new
                {
                    Name = library.Name,
                    Language = library.Language,
                    SupportsPeriodicals = library.SupportsPeriodicals,
                    PrimaryColor = library.PrimaryColor,
                    SecondaryColor = library.SecondaryColor,
                    OwnerEmail = library.OwnerEmail,
                    Public = library.Public,
                    DatabaseConnection = library.DatabaseConnection,
                    FileStoreType = library.FileStoreType,
                    FileStoreSource = library.FileStoreSource
                },
                cancellationToken: cancellationToken);
                libraryId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetLibraryById(libraryId, cancellationToken);
        }

        public async Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT l.*, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                            WHERE l.Id = @LibraryId";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<LibraryModel>(command);
            }
        }

        public async Task UpdateLibrary(LibraryModel library, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE Library
                            SET `Name` = @Name,
                                `Language` = @Language,
                                Description = @Description,
                                SupportsPeriodicals = @SupportsPeriodicals,
                                PrimaryColor = @PrimaryColor,
                                SecondaryColor = @SecondaryColor,
                                OwnerEmail = @OwnerEmail,
                                `Public` = @Public,
                                DatabaseConnection = @DatabaseConnection,
                                FileStoreType = @FileStoreType,
                                FileStoreSource = @FileStoreSource
                            WHERE Id = @Id";
                var command = new CommandDefinition(sql, new
                {
                    Id = library.Id,
                    Name = library.Name,
                    Description = library.Description,
                    Language = library.Language,
                    SupportsPeriodicals = library.SupportsPeriodicals,
                    PrimaryColor = library.PrimaryColor,
                    SecondaryColor = library.SecondaryColor,
                    OwnerEmail = library.OwnerEmail,
                    Public = library.Public,
                    DatabaseConnection = library.DatabaseConnection,
                    FileStoreType = library.FileStoreType,
                    FileStoreSource = library.FileStoreSource
                }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteLibrary(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"DELETE FROM Library WHERE Id = @Id";
                var command = new CommandDefinition(sql, new { Id = libraryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddAccountToLibrary(int accountId, int libraryId, Role role, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO AccountLibrary VALUES (@LibraryId, @AccountId, @Role)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId, Role = role }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task UpdateLibraryUser(LibraryUserModel model, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE AccountLibrary SET Role = @Role 
                            WHERE LibraryId = @LibraryId 
                                AND AccountId = @AccountId";
                var command = new CommandDefinition(sql, model, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task RemoveLibraryFromAccount(int libraryId, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"DELETE FROM AccountLibrary 
                            WHERE LibraryId = @LibraryId 
                                AND AccountId = @AccountId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<LibraryModel>> GetLibrariesByAccountId(int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role, f.Id As ImageId, f.FilePath AS ImageUrl
                            FROM Library l
                                LEFT OUTER JOIN `File` f ON f.Id = l.ImageId
                                INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId
                            ORDER BY l.Name";
                var command = new CommandDefinition(sql,
                                                    new { AccountId = accountId },
                                                    cancellationToken: cancellationToken);

                return await connection.QueryAsync<LibraryModel>(command);
            }
        }

        public async Task UpdateLibraryImage(int libraryId, long imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Library SET ImageId = @ImageId WHERE Id = @Id";
                var command = new CommandDefinition(sql, new { Id = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }
    }
}
