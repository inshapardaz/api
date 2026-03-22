using System.Data;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Adapters.Database.SqlServer;

public class SqlServerConnectionProvider(IOptions<Settings> settings) : IProvideConnection
{
    private readonly Settings _settings = settings.Value;

    public IDbConnection GetConnection() => new SqlConnection(_settings.Database.ConnectionString);
    public IDbConnection GetLibraryConnection() => GetConnection();

    // TODO : This will break the multiple database support. Fix it by using better connection management
    //public IDbConnection GetLibraryConnection() => new SqlConnection(_libraryConfiguration.ConnectionString);
}
