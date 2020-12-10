using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class AccountDataBuilder
    {
        private IDbConnection _connection;
        private Role _role;
        public AccountDto Account => Accounts.FirstOrDefault();
        public IEnumerable<AccountDto> Accounts { get; private set; }

        public AccountDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public AccountDataBuilder As(Role role)
        {
            _role = role;
            return this;
        }

        public AccountDto Build(int count = 1)
        {
            var fixture = new Fixture();
            Accounts = fixture.Build<AccountDto>()
                                 .With(a => a.Title, "Mr")
                                 .With(a => a.Email, Helpers.Random.Email)
                                 .With(a => a.Role, _role)
                                 .With(a => a.AcceptTerms, true)
                                 .With(a => a.Created, DateTime.Now)
                                 .CreateMany(count);

            _connection.AddAccounts(Accounts);

            return Account;
        }

        public void CleanUp()
        {
            if (Accounts != null)
                _connection.DeleteLibraries(Accounts.Select(l => l.Id));
        }
    }
}
