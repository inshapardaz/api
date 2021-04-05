using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public AccountRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<AccountModel>> FindAccounts(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                            FROM Account
                            WHERE s.FirstName LIKE @Query OR  s.LastName LIKE @Query OR s.Email LIKE @Query
                            ORDER BY FirstName, LastName
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql, new { Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<AccountModel>(command);

                var sqlAuthorCount = @"SELECT *
                                       FROM Account
                                       WHERE FirstName LIKE @Query OR  LastName LIKE @Query OR Email LIKE @Query
                                       ORDER BY Name";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<AccountModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<AccountModel>> GetAccounts(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *
                            FROM Accounts
                            Order By FirstName, LastName
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql, new { PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<AccountModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Accounts";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, cancellationToken: cancellationToken));

                return new Page<AccountModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<IEnumerable<AccountModel>> GetWriters(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select a.* from LibraryUser as lu
                            INNER JOIN Accounts as a on a.Id = lu.AccountId
                            WHERE lu.LibraryId = @LibraryId AND a.Role IN (0, 1, 2)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<AccountModel>(command);
            }
        }

        public async Task<IEnumerable<AccountModel>> FindWriters(int libraryId, string query, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select a.* from LibraryUser as lu
                            INNER JOIN Accounts as a on a.Id = lu.AccountId
                            WHERE lu.LibraryId = @LibraryId AND a.Role IN (0, 1, 2) AND (a.FirstName LIKE @Query OR a.LastName LIKE @Query)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Query = query }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<AccountModel>(command);
            }
        }
    }
}
