using System.Data;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Database.SqlServer;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class LibraryDataBuilder
    {
        private IDbConnection _connection;
        private bool _enablePeriodicals;
        public LibraryDto Library { get; private set; }

        public LibraryDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public LibraryDataBuilder WithPeriodicalsEnabled(bool periodicalsEnabled = true)
        {
            _enablePeriodicals = periodicalsEnabled;
            return this;
        }

        public LibraryDto Build()
        {
            var fixture = new Fixture();

            Library = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .With(l => l.SupportsPeriodicals, _enablePeriodicals)
                                 .Create();

            _connection.AddLibrary(Library);

            return Library;
        }

        public void CleanUp()
        {
            if (Library != null)
                _connection.DeleteLibrary(Library.Id);
        }
    }
}
