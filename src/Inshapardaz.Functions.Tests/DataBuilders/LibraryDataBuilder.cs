using System.Data;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class LibraryDataBuilder
    {
        private IDbConnection _connection;

        public LibraryDto Library { get; private set; }

        public LibraryDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public LibraryDto Build()
        {
            var fixture = new Fixture();

            Library = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
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
