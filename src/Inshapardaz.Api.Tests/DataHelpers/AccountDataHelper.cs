using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class AccountDataHelper
    {
        public static void AddAccounts(this IDbConnection connection, IEnumerable<AccountDto> accounts)
        {
            foreach (var account in accounts)
            {
                AddAccount(connection, account);
            }
        }

        public static void AddAccount(this IDbConnection connection, AccountDto account)
        {
            var sql = @"Insert Into Accounts (Title, FirstName, LastName, Email, AcceptTerms, Role, Created)
                        OUTPUT Inserted.Id VALUES (@Title, @FirstName, @LastName, @Email, @AcceptTerms, @Role, @Created)";
            var id = connection.ExecuteScalar<int>(sql, account);
            account.Id = id;
        }

        public static void DeleteAccounts(this IDbConnection connection, IEnumerable<int> accountIds)
        {
            connection.Execute("Delete Accounts Where Id IN @AccountIds", new { AccountIds = accountIds });
        }
    }
}
