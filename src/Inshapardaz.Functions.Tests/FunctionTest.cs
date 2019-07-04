using System;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
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

        public FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new Startup();
            
            DatabaseContext = CreateDbContext();
            
            _builder.Services.AddSingleton<IDatabaseContext>(sp => DatabaseContext);
            _builder.Services.AddSingleton<CategoriesDataBuilder>();
            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;

        protected IDatabaseContext DatabaseContext { get; private set; }

        protected void AuthenticateAsAdmin()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.AdminRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }

        protected void  AuthenticateAsWriter()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.WriteRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }

        protected void  AuthenticateAsReader()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.ReaderRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }
        
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
