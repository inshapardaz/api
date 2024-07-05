using FluentMigrator.Runner;
using Inshapardaz.Database.Migrations;
using Inshapardaz.Domain.Adapters.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using FluentMigrator.Runner.Initialization;

namespace Inshapardaz.Adapters.Database.MySql;

public class MySqlDatabaseMigration : IMigrateDatabase
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
                .AddMySql8()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Migrations).Assembly).For.Migrations())
            .Configure<RunnerOptions>(opt => { 
                opt.TransactionPerSession = true; })
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
