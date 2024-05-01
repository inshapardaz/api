using System;
using System.Data;
using System.Linq;
using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class AccountAssert
    {
        private readonly IDbConnection _connection;
        private AccountDto _account;

        public AccountAssert(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        internal AccountAssert AssertAccountExistsWithEmail(string adminEmail)
        {
            _account = _connection.GetAccountByEmail(adminEmail);

            _account.Should().NotBeNull();

            return this;
        }

        internal AccountAssert AssertAccountActive(int id)
        {
            _account = _connection.GetAccountById(id);
            _account.IsVerified.Should().BeTrue();
            _account.VerificationToken.Should().NotBeNullOrWhiteSpace();
            _account.Verified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            _account.InvitationCode.Should().BeNullOrWhiteSpace();
            _account.InvitationCodeExpiry.Should().BeNull();
            _account.AcceptTerms.Should().BeTrue();

            return this;
        }

        internal void AccountShouldNotExist(string ownerEmail, IDbConnection databaseConnection)
        {
            ownerEmail.Should().NotBeNull();
            var dbAccount = databaseConnection.GetAccountByEmail(ownerEmail);
            dbAccount.Should().BeNull();
        }

        internal AccountAssert AssertAccountHasResetToken(AccountDto account)
        {
            var dbAccount = _connection.GetAccountById(account.Id);
            dbAccount.Should().NotBeNull();
            dbAccount.ResetToken.Should().NotBeNull();
            dbAccount.ResetTokenExpires.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(2));

            return this;
        }

        internal AccountAssert WithName(string adminName)
        {
            _account.Name.Should().Be(adminName);
            return this;
        }

        internal AccountAssert ShouldBeInRole(Role role, int libraryId)
        {
            var libraries = _connection.GetAccountLibraries(_account.Id);
            libraries.SingleOrDefault(t => t.LibraryId == libraryId).Role.Should().Be(role);
            return this;
        }

        internal AccountAssert InNoLibrary()
        {
            var accounts = _connection.GetAccountLibraries(_account.Id);
            accounts.Should().BeEmpty();
            return this;
        }

        internal AccountAssert ShouldBeSuperAdmin()
        {
            _account.IsSuperAdmin.Should().BeTrue();
            return this;
        }

        internal AccountAssert ShouldBeInvited()
        {
            _account.InvitationCode.Should().NotBeNull();
            return this;
        }

        internal AccountAssert ShouldNotBeVerified()
        {
            _account.Verified.Should().BeNull();
            return this;
        }

        internal AccountAssert ShouldBeVerified()
        {
            _account.Verified.Should().NotBeNull();
            return this;
        }

        internal AccountAssert ShouldHaveInvitationExpiring(DateTime dateTime)
        {
            _account.InvitationCodeExpiry.Should().BeCloseTo(dateTime, TimeSpan.FromSeconds(2));
            return this;
        }

        internal void UserInLibrary(int accountId, int libraryId, Role? role)
        {
            var accounts = _connection.GetAccountLibraries(accountId);
            accounts.Should().Contain(t => t.LibraryId == libraryId, "user not found in library");
            if (role.HasValue)
            {
                accounts.Should().Contain(t => t.Role == role, "user with correct role not found in library");
            }
        }

        internal AccountAssert InLibrary(int libraryId)
        {
            var accounts = _connection.GetAccountLibraries(_account.Id);
            accounts.Should().Contain(t => t.LibraryId == libraryId, "user not found in library");
            return this;
        }
    }
}
