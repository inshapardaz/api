using System.Data;
using MySqlConnector;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Adapters.Database.MySql;

public class MySqlConnectionProvider : IProvideConnection
{
    private readonly Settings _settings;

    public MySqlConnectionProvider(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

    public IDbConnection GetConnection()
    {
        var connection = new MySqlConnection(_settings.Database.ConnectionString);
        connection.Open();
        return connection;
    }

    public IDbConnection GetLibraryConnection()
    {
        return GetConnection();

        // TODO : This will break the multiple database support. Fix it by using better connection management
        //var connection = new MySqlConnection(_libraryConfiguration.ConnectionString);
        //connection.Open();
        //return connection;
    }
}
