using Inshapardaz.Domain.Models;
using System.Data;
using System.Data.SqlClient;

namespace Inshapardaz.Database.SqlServer
{
    public class SqlServerConnectionProvider : IProvideConnection
    {
        private readonly string _connectionString;
        private readonly LibraryConfiguration _libraryConfiguration;

        public SqlServerConnectionProvider(string connectionString, LibraryConfiguration libraryConfiguration)
        {
            _connectionString = connectionString;
            _libraryConfiguration = libraryConfiguration;
        }

        public IDbConnection GetConnection() => new SqlConnection(_connectionString);
        public IDbConnection GetLibraryConnection() => new SqlConnection(_libraryConfiguration.ConnectionString ?? _connectionString);
    }
}
