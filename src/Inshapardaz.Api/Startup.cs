
using System;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Ports.Dictionary;
using Inshapardaz.Ports.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Paramore.Brighter;

namespace Inshapardaz.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public IConfiguration Configuration { get; }

        public Settings Settings { get; set; }

        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Settings = services.BindSettings(Configuration);

            services.AddRenderers()
                    .AddBrighterAndDarker()
                    .AddFramework()
                    .AddLucene()
                    .AddSwagger()
                    .AddCors()
                    .AddApiAuthentication(Settings)
                    .AddDatabase(CurrentEnvironment, Configuration)
                    .AddMappings()
                    .AddMvc();

            DatabaseModule.MigrateToDatabase(Configuration, services.BuildServiceProvider().GetService<IDatabaseContext>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine($"Configuring application with environment {env.EnvironmentName}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwaggerMiddleware()
               .UseCors()
               .UseDefaultFiles()
               .UseStaticFiles()
               .UseApiRedirection()
               .UseAuthentication()
               .UseStatusCodeMiddleWare()
               .UseTestAuthentication(CurrentEnvironment)
               .UseMvc();

            var commandProcessor = app.ApplicationServices.GetService<IAmACommandProcessor>();
            commandProcessor.SendAsync(new CreateDictionaryIndexRequest()).Wait(5 * 1000);
        }
    }
}