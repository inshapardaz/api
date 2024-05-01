using Inshapardaz.Domain.Models;
using System.Data;
using MySqlConnector;

namespace Inshapardaz.Database.MySql
{
    public class MySqlConnectionProvider : IProvideConnection
    {
        private readonly string _connectionString;
        private readonly LibraryConfiguration _libraryConfiguration;

        public MySqlConnectionProvider(string connectionString, LibraryConfiguration libraryConfiguration)
        {
            _connectionString = connectionString;
            _libraryConfiguration = libraryConfiguration;
        }

        public IDbConnection GetConnection() => new MySqlConnection(_connectionString).Open();
        public IDbConnection GetLibraryConnection() => new MySqlConnection(_libraryConfiguration.ConnectionString ?? _connectionString);
    }
}
