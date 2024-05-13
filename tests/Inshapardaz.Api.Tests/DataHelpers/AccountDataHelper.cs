using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class AccountDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;
        public static AccountDto GetAccountByEmail(this IDbConnection connection, string email)
        {
            var sql = @"SELECT * FROM Accounts WHERE Email = @Email";
            return connection.QuerySingleOrDefault<AccountDto>(sql, new { Email = email });
        }

        public static AccountDto GetAccountById(this IDbConnection connection, int id)
        {
            var sql = @"SELECT * FROM Accounts WHERE Id = @Id";
            return connection.QuerySingleOrDefault<AccountDto>(sql, new { Id = id });
        }

        public static void AddAccount(this IDbConnection connection, AccountDto account)
        {
            var sql = @"Insert Into Accounts (Name, Email, Passwordhash, AcceptTerms, IsSuperAdmin, Verified, InvitationCode, InvitationCodeExpiry, ResetToken, ResetTokenExpires, Created)
                        OUTPUT Inserted.Id VALUES (@Name, @Email, @Passwordhash, @AcceptTerms, @IsSuperAdmin, @Verified, @InvitationCode, @InvitationCodeExpiry, @ResetToken, @ResetTokenExpires, @Created);";
            var mySql = @"Insert Into Accounts (Name, Email, Passwordhash, AcceptTerms, IsSuperAdmin, Verified, InvitationCode, InvitationCodeExpiry, ResetToken, ResetTokenExpires, Created)
                VALUES (@Name, @Email, @Passwordhash, @AcceptTerms, @IsSuperAdmin, @Verified, @InvitationCode, @InvitationCodeExpiry, @ResetToken, @ResetTokenExpires, @Created);
                SELECT LAST_INSERT_ID();";
            var id = connection.ExecuteScalar<int>(_dbType == DatabaseTypes.SqlServer ? sql : mySql, account);
            account.Id = id;
        }

        public static void RevokeRefreshToken(this IDbConnection connection, string refreshToken)
        {
            var sql = @"UPDATE RefreshToken SET REVOKED = GETDATE() WHERE Token = @Token";
            var mySql = @"UPDATE RefreshToken SET REVOKED = SYSDATE() WHERE Token = @Token";
            connection.Execute(_dbType == DatabaseTypes.SqlServer ? sql : mySql, new { Token = refreshToken });
        }

        public static string GetRefreshToken(this IDbConnection connection, string email)
        {
            var sql = @"SELECT TOP 1 r.Token
                        FROM RefreshToken r
                        INNER JOIN Accounts a ON a.Id = r.AccountId
                        WHERE a.Email = @Email
                        ORDER BY r.Created";
            return connection.QuerySingle<string>(sql, new { Email = email });
        }

        public static void DeleteAccount(this IDbConnection connection, int accountId)
        {
            connection.Execute("Delete FROM Accounts Where Id = @AccountId", new { AccountId = accountId });
        }

        public static void DeleteAccountByEmail(this IDbConnection connection, string email)
        {
            connection.Execute("Delete FROM Accounts WHERE Email = @Email", new { Email = email });
        }

        public static void AddAccountToLibrary(this IDbConnection connection, AccountDto account, int libraryId, Role role = Role.Reader)
        {
            var sql = @"INSERT INTO AccountLibrary VALUES (@AccountId, @LibraryId, @Role)";
            connection.Execute(sql, new { AccountId = account.Id, LibraryId = libraryId, Role = role });
        }

        public static IEnumerable<AccountLibraryDto> GetAccountLibraries(this IDbConnection connection, int accountId)
        {
            var sql = @"SELECT * FROM AccountLibrary WHERE AccountId = @AccountId";
            return connection.Query<AccountLibraryDto>(sql, new { AccountId = accountId });
        }
    }
}
