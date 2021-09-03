using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class AccountDataBuilder
    {
        private IDbConnection _connection;

        private Role? _role = null;
        public string _password = RandomData.String;
        private DateTime? _verified = null;
        private string _invitationCode = null;
        private DateTime? _invitationExipry = null;
        private int? _libraryId;
        private string _resetToken = null;
        private DateTime? _resetTokenExpiry = null;

        public AccountDto Account { get; private set; }

        public AccountDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        internal AccountDataBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public AccountDataBuilder As(Role role)
        {
            _role = role;
            return this;
        }

        public AccountDataBuilder Verified()
        {
            _verified = DateTime.UtcNow;
            return this;
        }

        public AccountDataBuilder Unverified()
        {
            _verified = null;
            return this;
        }

        public AccountDataBuilder AsInvitation()
        {
            _invitationCode = Guid.NewGuid().ToString("N");
            _invitationExipry = DateTime.Today.AddDays(5);
            return this;
        }

        public AccountDataBuilder ExpiringInvitation(DateTime expirationDate)
        {
            _invitationExipry = expirationDate;
            return this;
        }

        public AccountDataBuilder InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        internal AccountDataBuilder WithResetToken(string resetToken)
        {
            _resetToken = resetToken;
            return this;
        }

        internal AccountDataBuilder WithResetTokenExpiry(DateTime expiry)
        {
            _resetTokenExpiry = expiry;
            return this;
        }

        public AccountDto Build()
        {
            var fixture = new Fixture();
            Account = fixture.Build<AccountDto>()
                                 .With(a => a.Email, RandomData.Email)
                                 .With(a => a.PasswordHash, SecretHasher.GetStringHash(_password))
                                 .With(a => a.Verified, _verified)
                                 .With(a => a.AcceptTerms, true)
                                 .With(a => a.Created, DateTime.UtcNow)
                                 .With(a => a.IsSuperAdmin, _role == Role.Admin)
                                 .With(a => a.InvitationCode, _invitationCode)
                                 .With(a => a.InvitationCodeExpiry, _invitationExipry)
                                 .With(a => a.ResetToken, _resetToken)
                                 .With(a => a.ResetTokenExpires, _resetTokenExpiry)
                                 .Create();

            _connection.AddAccount(Account);

            if (_role.HasValue && _role != Role.Admin && _libraryId.HasValue)
            {
                _connection.AddAccountToLibrary(Account, _libraryId.Value, _role.Value);
            }

            return Account;
        }

        public async Task<AuthenticateResponse> Authenticate(HttpClient client, string email = null)
        {
            if (Account != null)
            {
                var response = await client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = email ?? Account.Email, Password = _password });
                return await response.GetContent<AuthenticateResponse>();
            }

            throw new Exception("Account not found to authenticate. Please build an account before authenticating.");
        }

        public void CleanUp()
        {
            _connection.DeleteAccount(Account.Id);
        }
    }
}
