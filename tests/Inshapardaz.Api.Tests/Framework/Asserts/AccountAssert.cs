using System;
using System.Linq;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class AccountAssert
    {
        private readonly IAccountTestRepository _accountTestRepository;
        private AccountDto _account;

        public AccountAssert(IAccountTestRepository accountTestRepository)
        {
            _accountTestRepository = accountTestRepository;
        }

        public AccountAssert AssertAccountExistsWithEmail(string adminEmail)
        {
            _account = _accountTestRepository.GetAccountByEmail(adminEmail);

            _account.Should().NotBeNull();

            return this;
        }

        public AccountAssert AssertAccountActive(int id)
        {
            _account = _accountTestRepository.GetAccountById(id);
            _account.IsVerified.Should().BeTrue();
            _account.VerificationToken.Should().NotBeNullOrWhiteSpace();
            _account.Verified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            _account.InvitationCode.Should().BeNullOrWhiteSpace();
            _account.InvitationCodeExpiry.Should().BeNull();
            _account.AcceptTerms.Should().BeTrue();

            return this;
        }

        public void AccountShouldNotExist(string ownerEmail)
        {
            ownerEmail.Should().NotBeNull();
            var dbAccount = _accountTestRepository.GetAccountByEmail(ownerEmail);
            dbAccount.Should().BeNull();
        }

        public AccountAssert AssertAccountHasResetToken(AccountDto account)
        {
            var dbAccount = _accountTestRepository.GetAccountById(account.Id);
            dbAccount.Should().NotBeNull();
            dbAccount.ResetToken.Should().NotBeNull();
            dbAccount.ResetTokenExpires.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(2));

            return this;
        }

        public AccountAssert WithName(string adminName)
        {
            _account.Name.Should().Be(adminName);
            return this;
        }

        public AccountAssert ShouldBeInRole(Role role, int libraryId)
        {
            var libraries = _accountTestRepository.GetAccountLibraries(_account.Id);
            libraries.SingleOrDefault(t => t.LibraryId == libraryId).Role.Should().Be(role);
            return this;
        }

        public AccountAssert InNoLibrary()
        {
            var accounts = _accountTestRepository.GetAccountLibraries(_account.Id);
            accounts.Should().BeEmpty();
            return this;
        }

        public AccountAssert ShouldBeSuperAdmin()
        {
            _account.IsSuperAdmin.Should().BeTrue();
            return this;
        }

        public AccountAssert ShouldBeInvited()
        {
            _account.InvitationCode.Should().NotBeNull();
            return this;
        }

        public AccountAssert ShouldNotBeVerified()
        {
            _account.Verified.Should().BeNull();
            return this;
        }

        public AccountAssert ShouldBeVerified()
        {
            _account.Verified.Should().NotBeNull();
            return this;
        }

        public AccountAssert ShouldHaveInvitationExpiring(DateTime dateTime)
        {
            _account.InvitationCodeExpiry.Should().BeCloseTo(dateTime, TimeSpan.FromSeconds(2));
            return this;
        }

        public void UserInLibrary(int accountId, int libraryId, Role? role)
        {
            var accounts = _accountTestRepository.GetAccountLibraries(accountId);
            accounts.Should().Contain(t => t.LibraryId == libraryId, "user not found in library");
            if (role.HasValue)
            {
                accounts.Should().Contain(t => t.Role == role, "user with correct role not found in library");
            }
        }

        public AccountAssert InLibrary(int libraryId)
        {
            var accounts = _accountTestRepository.GetAccountLibraries(_account.Id);
            accounts.Should().Contain(t => t.LibraryId == libraryId, "user not found in library");
            return this;
        }
    }
}
