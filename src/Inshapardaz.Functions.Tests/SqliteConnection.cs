using Inshapardaz.Ports.Database;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Inshapardaz.Functions.Tests
{
    internal class SqliteConnectionProvider : IProvideConnection
    {
        private IDbConnection _connection = new SqliteConnection("DataSource=:memory:");

        public IDbConnection GetConnection() => _connection;
    }
}
