using Inshapardaz.Domain.Adapters.Repositories;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using System;
using Inshapardaz.Database.Migrations;

namespace Inshapardaz.Adapters.Database.SqlServer;

public class SqlServerDatabaseMigration : IMigrateDatabase
{
    public void UpdateDatabase(string connectionString, long? version = null)
    {
        var serviceProvider = CreateServices(connectionString);

        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider, version);
        }
    }

    private static IServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Migrations).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider, long? rollbackVersion = null)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        if (rollbackVersion.HasValue)
        {
            runner.MigrateDown(rollbackVersion.Value);
        }
        else
        {
            runner.MigrateUp();
        }
    }
}
