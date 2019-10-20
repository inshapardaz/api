using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.Repositories
{
    public abstract class BaseRepository
    {
        protected async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameter = null, CancellationToken token = default)
        {
            using (IDbConnection connection = DatabaseConnection.Connection)
            {
                return await connection.QueryAsync<T>(query, parameter);
            }
        }
        
        protected async Task<T> ExecuteScalar<T>(string query, object parameter = null, CancellationToken token = default)
        {
            using (IDbConnection connection = DatabaseConnection.Connection)
            {
                return await connection.ExecuteScalarAsync<T>(query, parameter);
            }
        }
    }
}
