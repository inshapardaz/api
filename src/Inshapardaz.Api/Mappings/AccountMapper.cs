using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Views.Accounts;

namespace Inshapardaz.Api.Mappings
{
    public static class AccountMapper
    {
        public static AccountView Map(this AccountModel source)
            => new AccountView
            {
                Id = source.Id,
                AccountName = source.FirstName,
                Email = source.Email,
                IsVerified = source.IsVerified,
                Created = source.Created,
                Role = source.Role,
                Updated = source.Updated
            };

        public static AccountModel Map(this AccountView source)
            => new AccountModel
            {
                Id = source.Id,
                FirstName = source.AccountName,
                Email = source.Email,
                IsVerified = source.IsVerified,
                Created = source.Created,
                Role = source.Role,
                Updated = source.Updated
            };

        public static AccountLookupView MapToLookup(this AccountModel source)
            => new AccountLookupView
            {
                Id = source.Id,
                AccountName = $"{source.FirstName} {source.LastName}"
            };
    }
}
