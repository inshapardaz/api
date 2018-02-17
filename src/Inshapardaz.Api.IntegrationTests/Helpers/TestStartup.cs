using System;
using Inshapardaz.Domain;
using Microsoft.AspNetCore.Builder;
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
