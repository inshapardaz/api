using System;
using Inshapardaz.Domain;
using Inshapardaz.Ports.Database;
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

        protected override void ConfigureDomain(IServiceCollection services)
        {
            base.ConfigureDomain(services);
            return;
            var databaseName = Guid.NewGuid().ToString();

            services.AddEntityFrameworkSqlite()
                    .AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName));

            DatabaseModule.ConfigureDatabase(services, Configuration);
        }

        protected override void ConfigureSettings(IServiceCollection services)
        {
            Settings = new Settings();
            Settings.ElasticsearchUrl = "http://localhost:9200";
            services.AddSingleton(Settings);
        }

        protected override void ConfigureCustomMiddleWare(IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }
    }
}
