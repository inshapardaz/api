using System;
using Inshapardaz.Domain.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) 
            : base(configuration)
        {
        }

        protected override void AddHangFire(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Console.WriteLine(context.Request.Path.Value);
                await next();
            });
        }

        protected override void ConfigureMvc(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
        }

        protected override void ConfigureHangFire(IServiceCollection services)
        {
        }

        protected override void ConfigureCustoMiddleWare(IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }

        protected override void ConfigureDomain(IServiceCollection services)
        {
            var databaseName = Guid.NewGuid().ToString();

            services.AddEntityFrameworkSqlite()
                    .AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName));

            services.AddTransient<IDatabaseContext, DatabaseContext>();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName);

            using (var context = new DatabaseContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
