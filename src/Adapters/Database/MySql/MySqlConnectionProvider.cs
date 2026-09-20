using System.Data;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using MySql.Data.MySqlClient;

namespace Inshapardaz.Adapters.Database.MySql;

public class MySqlConnectionProvider(IOptions<Settings> settings, LibraryConfiguration libraryConfiguration) : IProvideConnection
{
    private readonly Settings _settings = settings.Value;

    public IDbConnection GetConnection()
    {
        var connection = new MySqlConnection(_settings.Database.ConnectionString);
        connection.Open();
        return connection;
    }

    public IDbConnection GetLibraryConnection()
    {
        var connectionString = string.IsNullOrWhiteSpace(libraryConfiguration.ConnectionString)
            ? _settings.Database.ConnectionString
            : libraryConfiguration.ConnectionString;

        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
