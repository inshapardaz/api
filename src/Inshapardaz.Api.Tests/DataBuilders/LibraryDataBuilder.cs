using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Database.SqlServer;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class LibraryDataBuilder
    {
        private IDbConnection _connection;
        private bool _enablePeriodicals;
        private int? _accountId;

        public LibraryDto Library => Libraries.FirstOrDefault();
        public IEnumerable<LibraryDto> Libraries { get; private set; }

        public LibraryDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public LibraryDataBuilder WithPeriodicalsEnabled(bool periodicalsEnabled = true)
        {
            _enablePeriodicals = periodicalsEnabled;
            return this;
        }

        internal LibraryDataBuilder AssignToUser(int? accountId)
        {
            _accountId = accountId;
            return this;
        }

        public LibraryDto Build(int count = 1)
        {
            // TODO Add
            var fixture = new Fixture();
            Libraries = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .With(l => l.SupportsPeriodicals, _enablePeriodicals)
                                 .CreateMany(count);

            _connection.AddLibraries(Libraries);

            if (_accountId.HasValue)
            {
                _connection.AssignLibrariesToUser(Libraries, _accountId);
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
