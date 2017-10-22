using Inshapardaz.Domain.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.IntegrationTests
{
    class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) 
            : base(configuration)
        {
        }

        protected override void AddHangFire(IApplicationBuilder app)
        {
        }

        protected override void ConfigureHangFire(IServiceCollection services)
        {
        }

        protected override void ConfigureDomain(IServiceCollection services)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);


            services.AddEntityFrameworkSqlite()
                    .AddDbContext<DatabaseContext>(options => options.UseSqlite(connection));

            services.AddTransient<IDatabaseContext, DatabaseContext>();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlite(connectionString);

            var database = new DatabaseContext(optionsBuilder.Options).Database;
            database.EnsureCreated();
        }
    }
}
