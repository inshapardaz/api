using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IAccountTestRepository
    {
        AccountDto GetAccountByEmail(string email);
        AccountDto GetAccountById(int id);
        void AddAccount(AccountDto account);
        void RevokeRefreshToken(string refreshToken);
        string GetRefreshToken(string email);

        void DeleteAccount(int accountId);
        void DeleteAccountByEmail(string email);
        void AddAccountToLibrary(AccountDto account, int libraryId, Role role = Role.Reader);
        IEnumerable<AccountLibraryDto> GetAccountLibraries(int accountId);
    }

    public class MySqlAccountTestRepository : IAccountTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlAccountTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public AccountDto GetAccountByEmail(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Accounts WHERE Email = @Email";
                return connection.QuerySingleOrDefault<AccountDto>(sql, new { Email = email });
            }
        }

        public AccountDto GetAccountById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Accounts WHERE Id = @Id";
                return connection.QuerySingleOrDefault<AccountDto>(sql, new { Id = id });
            }
        }

        public void AddAccount(AccountDto account)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Accounts (Name, Email, Passwordhash, AcceptTerms, IsSuperAdmin, Verified, InvitationCode, InvitationCodeExpiry, ResetToken, ResetTokenExpires, Created)
                VALUES (@Name, @Email, @Passwordhash, @AcceptTerms, @IsSuperAdmin, @Verified, @InvitationCode, @InvitationCodeExpiry, @ResetToken, @ResetTokenExpires, @Created);
                SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, account);
                account.Id = id;
            }
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE RefreshToken SET REVOKED = SYSDATE() WHERE Token = @Token";
                connection.Execute(sql, new { Token = refreshToken });
            }
        }

        public string GetRefreshToken(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT TOP 1 r.Token
                        FROM RefreshToken r
                        INNER JOIN Accounts a ON a.Id = r.AccountId
                        WHERE a.Email = @Email
                        ORDER BY r.Created";
                return connection.QuerySingle<string>(sql, new { Email = email });
            }
        }

        public void DeleteAccount(int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            { connection.Execute("Delete FROM Accounts Where Id = @AccountId", new { AccountId = accountId }); }
        }

        public void DeleteAccountByEmail(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                connection.Execute("Delete FROM Accounts WHERE Email = @Email", new { Email = email });
            }
        }

        public void AddAccountToLibrary(AccountDto account, int libraryId, Role role = Role.Reader)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO AccountLibrary VALUES (@AccountId, @LibraryId, @Role)";
                connection.Execute(sql, new { AccountId = account.Id, LibraryId = libraryId, Role = role });
            }
        }

        public IEnumerable<AccountLibraryDto> GetAccountLibraries(int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM AccountLibrary WHERE AccountId = @AccountId";
                return connection.Query<AccountLibraryDto>(sql, new { AccountId = accountId });
            }
        }
    }

    public class SqlServerAccountTestRepository : IAccountTestRepository
    {
        private IProvideConnection _connectionProvider;
        public SqlServerAccountTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }


        public AccountDto GetAccountByEmail(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Accounts WHERE Email = @Email";
                return connection.QuerySingleOrDefault<AccountDto>(sql, new { Email = email });
            }
        }

        public AccountDto GetAccountById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Accounts WHERE Id = @Id";
                return connection.QuerySingleOrDefault<AccountDto>(sql, new { Id = id });
            }
        }

        public void AddAccount(AccountDto account)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Accounts (Name, Email, Passwordhash, AcceptTerms, IsSuperAdmin, Verified, InvitationCode, InvitationCodeExpiry, ResetToken, ResetTokenExpires, Created)
                        OUTPUT Inserted.Id VALUES (@Name, @Email, @Passwordhash, @AcceptTerms, @IsSuperAdmin, @Verified, @InvitationCode, @InvitationCodeExpiry, @ResetToken, @ResetTokenExpires, @Created);";

                var id = connection.ExecuteScalar<int>(sql, account);
                account.Id = id;
            }
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE RefreshToken SET REVOKED = GETDATE() WHERE Token = @Token";
                connection.Execute(sql, new { Token = refreshToken });
            }
        }

        public string GetRefreshToken(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT TOP 1 r.Token
                        FROM RefreshToken r
                        INNER JOIN Accounts a ON a.Id = r.AccountId
                        WHERE a.Email = @Email
                        ORDER BY r.Created";
                return connection.QuerySingle<string>(sql, new { Email = email });
            }
        }

        public void DeleteAccount(int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            { connection.Execute("Delete FROM Accounts Where Id = @AccountId", new { AccountId = accountId }); }
        }

        public void DeleteAccountByEmail(string email)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                connection.Execute("Delete FROM Accounts WHERE Email = @Email", new { Email = email });
            }
        }

        public void AddAccountToLibrary(AccountDto account, int libraryId, Role role = Role.Reader)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO AccountLibrary VALUES (@AccountId, @LibraryId, @Role)";
                connection.Execute(sql, new { AccountId = account.Id, LibraryId = libraryId, Role = role });
            }
        }

        public IEnumerable<AccountLibraryDto> GetAccountLibraries(int accountId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM AccountLibrary WHERE AccountId = @AccountId";
                return connection.Query<AccountLibraryDto>(sql, new { AccountId = accountId });
            }
        }
    }
}
