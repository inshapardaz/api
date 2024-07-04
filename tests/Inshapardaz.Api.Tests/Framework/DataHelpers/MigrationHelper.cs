using System;
using System.IO;
using System.Linq;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Inshapardaz.Database.Migrations;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public class MigrationHelper
    {
        private readonly IServiceProvider _serviceProvider;
        public MigrationHelper()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var databaseType = config.GetSection("AppSettings").GetValue<DatabaseTypes>("Database:DatabaseConnectionType");
            var connectionString = config.GetSection("AppSettings").GetValue<string>("Database:ConnectionString");

            _serviceProvider = new ServiceCollection()
                                    .AddFluentMigratorCore()
                                    .ConfigureRunner(builder =>
                                    {
                                        if (databaseType == DatabaseTypes.MySql)
                                        {
                                            builder.AddMySql8()
                                                .ScanIn(typeof(Migrations).Assembly)
                                                .For.Migrations()
                                                .ConfigureGlobalProcessorOptions(x => x.ConnectionString = connectionString);
                                        }
                                        else if (databaseType == DatabaseTypes.SqlServer)
                                        {
                                            builder.AddSqlServer()
                                                .ScanIn(typeof(Migrations).Assembly)
                                                .For.Migrations()
                                                .ConfigureGlobalProcessorOptions(x => x.ConnectionString = connectionString);
                                        }
                                    })
                                    .Configure<RunnerOptions>(ro =>
                                    {
                                        ro.Tags = new[] { "qa" };
                                        ro.TransactionPerSession = false;
                                    })
                                    .AddLogging(l => l.AddFluentMigratorConsole())
                                    .BuildServiceProvider();
        }
        public void RunUpAndDown(long migrateFromVersion, long migrateToVersion)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp(migrateToVersion);
                runner.MigrateDown(migrateFromVersion);
            }
        }

        public void RunUpTo(long version)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp(version);
            }
        }

        public void RunDownTo(long version)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateDown(version);
            }
        }

        public MigrationInfo GetMigrationInfo()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var result = new MigrationInfo();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                result.InAssembly = runner.MigrationLoader.LoadMigrations().ToList();
                result.LatestAssemblyVersion = result.InAssembly.Max(x => x.Key);
                var migrationIdsInTheDatabase = scope
                                    .ServiceProvider
                                    .GetRequiredService<IVersionLoader>()
                                    .VersionInfo
                                    .AppliedMigrations()
                                    .ToList();
                result.InDatabase = migrationIdsInTheDatabase
                        .Select(o => result
                                        .InAssembly
                                        .FirstOrDefault(x => x.Value.Version == o))
                        .ToList();
                var assemblyKeys = result.InAssembly
                        .Select(x => x.Key).ToList();
                var missingDatabaseMigrations = assemblyKeys
                        .Where(x => migrationIdsInTheDatabase.Contains(x) == false)
                        .ToList();
                result.NotInDatabase = missingDatabaseMigrations
                        .Select(o => result.InAssembly.First(x => x.Value.Version == o))
                        .ToList();
                result.LatestDatabaseVersion = migrationIdsInTheDatabase.Any() ? migrationIdsInTheDatabase.Max(x => x) : 0;

                return result;
            }
        }
    }
}
