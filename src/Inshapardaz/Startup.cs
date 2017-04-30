using Inshapardaz.Api.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = env.InitialiseConfiguration();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterFramework(Configuration)
                   .RegisterRenderes()
                   .RegisterDomain(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.ConfigureLogging(Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWelcomePage("/welcome");
            app.ConfigureApiAuthentication(Configuration)
                .ConfigureApplication()
               .ConfigureObjectMappings();
        }
    }
}
