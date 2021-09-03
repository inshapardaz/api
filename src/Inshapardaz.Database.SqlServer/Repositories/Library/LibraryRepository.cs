using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public LibraryRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<LibraryModel>> GetLibraries(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role
                            FROM Library l
                            LEFT OUTER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            Order By l.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { PageSize = pageSize, PageNumber = pageNumber },
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
                var sql = @"SELECT  l.*, al.Role
                            FROM Library l
                            LEFT OUTER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE l.Name LIKE @Query
                            Order By l.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library WHERE Name LIKE @Query";
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
                var sql = @"SELECT  l.*, al.Role
                            FROM Library l
                            INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId
                            Order By l.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { AccountId = accountId, PageSize = pageSize, PageNumber = pageNumber },
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
                var sql = @"SELECT  l.*, al.Role
                            FROM Library l
                            INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId AND Name LIKE @Query
                            Order By l.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", AccountId = accountId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                            FROM Library l
                            INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId AND Name LIKE @Query";
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

        public async Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken)
        {
            int libraryId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Library(Name, Language, SupportsPeriodicals, PrimaryColor, SecondaryColor) OUTPUT Inserted.Id VALUES(@Name, @Language, @SupportsPeriodicals, @PrimaryColor, @SecondaryColor);";
                var command = new CommandDefinition(sql, new
                {
                    Name = library.Name,
                    Language = library.Language,
                    SupportsPeriodicals = library.SupportsPeriodicals,
                    PrimaryColor = library.PrimaryColor,
                    SecondaryColor = library.SecondaryColor
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
                var sql = @"SELECT l.*
                            FROM Library l
                            Where Id = @LibraryId";
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
                var sql = @"Update Library Set Name = @Name,
                            Language = @Language,
                            SupportsPeriodicals = @SupportsPeriodicals,
                            PrimaryColor = @PrimaryColor,
                            SecondaryColor = @SecondaryColor
                            Where Id = @Id";
                var command = new CommandDefinition(sql, new
                {
                    Id = library.Id,
                    Name = library.Name,
                    Language = library.Language,
                    SupportsPeriodicals = library.SupportsPeriodicals,
                    PrimaryColor = library.PrimaryColor,
                    SecondaryColor = library.SecondaryColor
                }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteLibrary(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library Where Id = @Id";
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

        public async Task RemoveLibraryFromAccount(int libraryId, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From AccountLibrary Where LibraryId = @LibraryId AND AccountId = @AccountId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<LibraryModel>> GetLibrariesByAccountId(int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  l.*, al.Role
                            FROM Library l
                            INNER JOIN AccountLibrary al ON al.LibraryId = l.Id
                            WHERE al.AccountId = @AccountId
                            Order By l.Name";
                var command = new CommandDefinition(sql,
                                                    new { AccountId = accountId },
                                                    cancellationToken: cancellationToken);

                return await connection.QueryAsync<LibraryModel>(command);
            }
        }
    }
}
