using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Models;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class LibraryDataBuilder
    {
        private IDbConnection _connection;
        private bool _enablePeriodicals;
        private int? _accountId;
        private Role _role;
        private string _startWith;

        public LibraryDto Library => Libraries.FirstOrDefault();

        public IEnumerable<LibraryDto> Libraries { get; private set; }

        public LibraryDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        internal LibraryDataBuilder StartingWith(string startWith)
        {
            _startWith = startWith;
            return this;
        }

        public LibraryDataBuilder WithPeriodicalsEnabled(bool periodicalsEnabled = true)
        {
            _enablePeriodicals = periodicalsEnabled;
            return this;
        }

        internal LibraryDataBuilder AssignToUser(int accountId, Role role = Role.Reader)
        {
            _accountId = accountId;
            _role = role;
            return this;
        }

        internal LibraryDataBuilder WithOutAccount()
        {
            _accountId = null;
            return this;
        }

        public LibraryDto Build(int count = 1)
        {
            var fixture = new Fixture();
            Libraries = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .With(l => l.SupportsPeriodicals, _enablePeriodicals)
                                 .With(l => l.Name, _startWith ?? RandomData.Name)
                                 .With(l => l.PrimaryColor, RandomData.String)
                                 .With(l => l.SecondaryColor, RandomData.String)
                                 .With(l => l.OwnerEmail, RandomData.Email)
                                 .CreateMany(count);

            _connection.AddLibraries(Libraries);

            if (_accountId.HasValue)
            {
                _connection.AssignLibrariesToUser(Libraries, _accountId.Value, _role);
            }

            return Library;
        }

        public void CleanUp()
        {
            if (Libraries != null)
                _connection.DeleteLibraries(Libraries.Select(l => l.Id));
        }
    }
}
