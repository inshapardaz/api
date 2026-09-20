using System.Data;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Adapters.Database.SqlServer;

public class SqlServerConnectionProvider(IOptions<Settings> settings, LibraryConfiguration libraryConfiguration) : IProvideConnection
{
    private readonly Settings _settings = settings.Value;

    public IDbConnection GetConnection() => new SqlConnection(_settings.Database.ConnectionString);

    public IDbConnection GetLibraryConnection() => new SqlConnection(
        string.IsNullOrWhiteSpace(libraryConfiguration.ConnectionString)
            ? _settings.Database.ConnectionString
            : libraryConfiguration.ConnectionString);
}
