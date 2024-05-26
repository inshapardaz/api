namespace Inshapardaz.Domain.Adapters.Repositories;

public interface IMigrateDatabase
{
    public void UpdateDatabase(string connectionString, long? version = null);
}
