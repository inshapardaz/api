using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Infrastructure.Factories;

public class DatabaseMigrationFactory
{
    public IMigrateDatabase CreateMigrator(DatabaseTypes? types)
    {
        return types switch
        {
            DatabaseTypes.MySql => new MySqlDatabaseMigration(),
            DatabaseTypes.SqlServer => new SqlServerDatabaseMigration(),
            _ => throw new ArgumentOutOfRangeException(nameof(types)),
        };
    }
}
