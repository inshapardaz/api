using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public AccountRepository(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<AccountModel>> FindAccounts(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                            FROM Accounts
                            WHERE Name LIKE @Query OR  Email LIKE @Query
                            ORDER BY Name
                            LIMIT @PageSize
                            OFFSET @PageSize * (@PageNumber - 1)";
                var command = new CommandDefinition(sql, new { Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<AccountModel>(command);

                var sqlAuthorCount = @"SELECT *
                                       FROM Accounts
                                       WHERE Name LIKE @Query OR Email LIKE @Query
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
                            Order By Name
                            LIMIT @PageSize
                            OFFSET @PageSize * (@PageNumber - 1)";
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

        public async Task<Page<AccountModel>> GetAccountsByLibrary(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  a.*, al.Role AS Role
                            FROM Accounts a
                            INNER JOIN AccountLibrary as al on a.Id = al.AccountId
                            WHERE al.LibraryId = @LibraryId
                            Order By a.Name
                            LIMIT @PageSize
                            OFFSET @PageSize * (@PageNumber - 1);";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<AccountModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                            FROM Accounts a
                            INNER JOIN AccountLibrary as al on a.Id = al.AccountId
                            WHERE al.LibraryId = @LibraryId";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<AccountModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<AccountModel>> FindAccountsByLibrary(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.*, al.Role AS Role
                            FROM Accounts a
                            INNER JOIN AccountLibrary AS al ON a.Id = al.AccountId
                            WHERE al.LibraryId = @LibraryId
                            AND (a.Name LIKE @Query OR a.Email LIKE @Query)
                            ORDER BY a.Name
                            LIMIT @PageSize
                            OFFSET @PageSize * (@PageNumber - 1)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<AccountModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*)
                                        FROM Accounts a
                                        INNER JOIN AccountLibrary AS al ON a.Id = al.AccountId
                                        WHERE al.LibraryId = @LibraryId
                                        AND (a.Name LIKE @Query OR a.Email LIKE @Query)";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, Query = $"%{query}%" }, cancellationToken: cancellationToken));

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
                var sql = @"Select a.* from Accounts as a
                            INNER JOIN AccountLibrary as al on a.Id = al.AccountId
                            WHERE al.LibraryId = @LibraryId AND al.Role IN (0, 1, 2)";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<AccountModel>(command);
            }
        }

        public async Task<IEnumerable<AccountModel>> FindWriters(int libraryId, string query, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select a.* from Accounts as a
                            INNER JOIN AccountLibrary as al on a.Id = al.AccountId
                            WHERE al.LibraryId = @LibraryId AND al.Role IN (0, 1, 2) AND a.Name LIKE @Query";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Query = query }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<AccountModel>(command);
            }
        }

        public async Task<int> AddInvitedAccount(string name, string email, Role role, string invitationCode, System.DateTime invitationCodeExpiry, int? libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Accounts (Name, Email, InvitationCode, InvitationCodeExpiry, IsSuperAdmin, AcceptTerms, Created)
                            VALUES (@Name, @Email, @InvitationCode, @InvitationCodeExpiry, @IsSuperAdmin, 1, UTC_TIMESTAMP());
                            SELECT LAST_INSERT_ID();";

                var command = new CommandDefinition(sql, new
                {
                    Name = name,
                    Email = email,
                    IsSuperAdmin = role == Role.Admin,
                    InvitationCode = invitationCode,
                    InvitationCodeExpiry = invitationCodeExpiry
                }, cancellationToken: cancellationToken);

                var accountId = await connection.ExecuteScalarAsync<int>(command);

                if (libraryId.HasValue)
                {
                    var sql2 = "Insert Into AccountLibrary VALUES (@AccountId, @LibraryId, @Role)";
                    var command2 = new CommandDefinition(sql2, new
                    {
                        AccountId = accountId,
                        LibraryId = libraryId,
                        Role = role
                    }, cancellationToken: cancellationToken);

                    await connection.ExecuteAsync(command2);
                }

                return accountId;
            }
        }

        public async Task<AccountModel> GetAccountByInvitationCode(string invitationCode, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select * from Accounts WHERE InvitationCode = @InvitationCode";
                var command = new CommandDefinition(sql, new { InvitationCode = invitationCode }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AccountModel>(command);
            }
        }

        public async Task<AccountModel> GetAccountByEmail(string email, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select * from Accounts WHERE Email = @Email";
                var command = new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AccountModel>(command);
            }
        }

        public async Task<AccountModel> AddAccount(AccountModel account, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Accounts (
                    Name, 
                    Email, 
                    IsSuperAdmin, 
                    InvitationCode, 
                    InvitationCodeExpiry, 
                    Created, 
                    AcceptTerms, 
                    Updated, 
                    PasswordHash, 
                    PasswordReset, 
                    ResetToken, 
                    ResetTokenExpires, 
                    VerificationToken, 
                    Verified)
                VALUES (
                    @Name, 
                    @Email, 
                    @IsSuperAdmin, 
                    @InvitationCode, 
                    @InvitationCodeExpiry, 
                    @Created,
                    @AcceptTerms, 
                    @Updated, 
                    @PasswordHash,
                    @PasswordReset,
                    @ResetToken,
                    @ResetTokenExpires,
                    @VerificationToken,
                    @Verified);
                SELECT LAST_INSERT_ID();";

                var command = new CommandDefinition(sql, new
                {
                    account.Name,
                    account.Email,
                    account.IsSuperAdmin,
                    account.InvitationCode,
                    account.InvitationCodeExpiry,
                    account.Created,
                    account.AcceptTerms,
                    account.Updated,
                    account.PasswordHash,
                    account.PasswordReset,
                    account.ResetToken,
                    account.ResetTokenExpires,
                    account.VerificationToken,
                    account.Verified
                }, cancellationToken: cancellationToken);

                var accountId = await connection.ExecuteScalarAsync<int>(command);
                account.Id = accountId;
                return account;
            }
        }

        public async Task AddAccountToLibrary(int libraryId, int accountId, Role role, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into AccountLibrary VALUES (@AccountId, @LibraryId, @Role)";
                var command = new CommandDefinition(sql, new
                {
                    AccountId = accountId,
                    LibraryId = libraryId,
                    Role = role
                }, cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
        }
        public async Task UpdateInvitationCode(string email, string invitationCode, System.DateTime invitationCodeExpiry, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Update Accounts SET InvitationCode = @InvitationCode, InvitationCodeExpiry = @InvitationCodeExpiry WHERE Email = @Email";
                var command = new CommandDefinition(sql, new { InvitationCode = invitationCode, InvitationCodeExpiry = invitationCodeExpiry, Email = email }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Role> GetUserRole(int accountId, int libraryId, CancellationToken cancellationToken = default)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT AT.* FROM AccountLibrary AL
                            INNER JOIN Accounts a ON a.Id = AL.AccountId
                            INNER JOIN Library t ON t.id = AL.LibraryId
                            WHERE AL.AccountId = @AccountId AND AL.LibraryId = @LibraryId";

                var command = new CommandDefinition(sql, new { AccountId = accountId, LibraryId = libraryId }, cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<Role>(command);
            }
        }

        public async Task<AccountModel> GetAccountById(int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select * from Accounts WHERE Id = @AccountId";
                var command = new CommandDefinition(sql, new { AccountId = accountId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AccountModel>(command);
            }
        }

        public async Task<AccountModel> GetLibraryAccountById(int libraryId, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select a.*, al.Role as Role
                            FROM Accounts a
                            INNER JOIN AccountLibrary as al on a.Id = al.AccountId
                            WHERE Id = @AccountId AND al.LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, AccountId = accountId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AccountModel>(command);
            }
        }

        public async Task<RefreshTokenModel> GetRefreshToken(string token, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM RefreshToken
                            WHERE Token = @Token";
                var command = new CommandDefinition(sql, new { Token = token }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<RefreshTokenModel>(command);
            }
        }

        public async Task AddRefreshToken(RefreshTokenModel refreshToken, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO RefreshToken
                (AccountId, Token, Expires, Created, CreatedByIp, Revoked, RevokedByIp, ReplacedByToken)
                VALUES
                (@AccountId, @Token, @Expires, @Created, @CreatedByIp, @Revoked, @RevokedByIp, @ReplacedByToken)";
                var command = new CommandDefinition(sql, new
                {
                    AccountId = accountId,
                    refreshToken.Token,
                    refreshToken.Expires,
                    refreshToken.Created,
                    refreshToken.CreatedByIp,
                    refreshToken.Revoked,
                    refreshToken.RevokedByIp,
                    refreshToken.ReplacedByToken
                }, cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
        }

        public async Task RemoveOldRefreshTokens(AccountModel account, int tokenAgeInDays, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @$"DELETE FROM RefreshToken 
                            WHERE AccountId = @AccountId AND Revoked IS NULL AND Created < (CURDATE() - INTERVAL {tokenAgeInDays} DAY)";
                var command = new CommandDefinition(sql, new { AccountId = account.Id }, cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
        }

        public async Task RevokeRefreshToken(string refreshToken, string ipAddress, string newRefreshToken, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE RefreshToken
                            SET Revoked = UTC_TIMESTAMP(),
                            RevokedByIp = @RevokedByIp,
                            ReplacedByToken = @ReplacedByToken
                            WHERE Token = @Token";
                var command = new CommandDefinition(sql, new { Token = refreshToken, RevokedByIp = ipAddress, ReplacedByToken = newRefreshToken }, cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
        }

        public async Task<AccountModel> GetAccountByResetToken(string token, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select * from Accounts WHERE ResetToken = @ResetToken";
                var command = new CommandDefinition(sql, new { ResetToken = token }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AccountModel>(command);
            }
        }

        public async Task UpdateAccount(AccountModel account, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE Accounts
                            SET Name = @Name,
                            PasswordHash = @PasswordHash,
                            AcceptTerms = @AcceptTerms,
                            IsSuperAdmin = @IsSuperAdmin,
                            InvitationCode = @InvitationCode,
                            InvitationCodeExpiry = @InvitationCodeExpiry,
                            VerificationToken  = @VerificationToken,
                            Verified = @Verified,
                            ResetToken = @ResetToken,
                            ResetTokenExpires = @ResetTokenExpires,
                            PasswordReset = @PasswordReset
                            WHERE Email = @Email";
                var command = new CommandDefinition(sql, account, cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
        }
    }
}
