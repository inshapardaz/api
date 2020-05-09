using System;
using System.Data;
using FluentMigrator.Runner;
using Inshapardaz.Ports.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public abstract class FunctionTest
    {
        private readonly TestHostBuilder _builder;
        private readonly TestStartup _startup;

        protected FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new TestStartup();

            InitializeDatabaseMigration(_builder.Services);
            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;

        protected IDbConnection DatabaseConnection => Container.GetService<IProvideConnection>().GetConnection();

        protected void InitializeDatabaseMigration(IServiceCollection services)
        {
            var serviceProvider = services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(TestSqliteConnectionProvider.ConnectionString)
                    .ScanIn(typeof(Database.Migrations.Migration000001_CreateLibrarySchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        protected virtual void Cleanup()
        {
        }
    }
}
