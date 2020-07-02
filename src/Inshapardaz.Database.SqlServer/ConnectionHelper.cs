using System.Data;
using System.Data.SqlClient;

namespace Inshapardaz.Database.SqlServer
{
    public class SqlServerConnectionProvider : IProvideConnection
    {
        private readonly string _connectionString;

        public SqlServerConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection() => new SqlConnection(_connectionString);
    }
}
