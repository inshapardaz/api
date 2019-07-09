using System;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Ports.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public abstract class FunctionTest
    {
        private readonly TestHostBuilder _builder;
        private readonly Startup _startup;
        private SqliteConnection _connection;

        protected FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new Startup();
            
            DatabaseContext = CreateDbContext();
            
            _builder.Services.AddSingleton<IDatabaseContext>(sp => DatabaseContext);
            _builder.Services.AddTransient<CategoriesDataBuilder>()
                             .AddTransient<SeriesDataBuilder>()
                             .AddTransient<AuthorsDataBuilder>()
                             .AddTransient<BooksDataBuilder>()
                             .AddSingleton<IFileStorage, FakeFileStorage>();

            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;

        protected IDatabaseContext DatabaseContext { get; private set; }

        
        private IDatabaseContext CreateDbContext()
        {
            if (_connection != null)
            {
                throw new Exception("connection already created");
            }
            
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                          .UseSqlite(_connection)
                          .EnableSensitiveDataLogging()
                          .EnableDetailedErrors()
                          .Options;

             var context = new DatabaseContext(options);
             context.Database.EnsureCreated();
             return context;
        }

        protected void Cleanup()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
