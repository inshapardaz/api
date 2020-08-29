using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new Settings();
            Configuration.Bind("Application", settings);
            services.AddSingleton(settings);

            services.AddControllers();
            services.AddBrighterCommand();
            services.AddDarkerQuery();

            AddDatabaseConnection(services)
                .AddDatabase();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IFileStorage, FileStorage>();
            services.AddTransient<IUserHelper, UserHelper>();
            services.AddTransient<IRenderAuthor, AuthorRenderer>();
            services.AddTransient<IRenderBook, BookRenderer>();
            services.AddTransient<IRenderCategory, CategoryRenderer>();
            services.AddTransient<IRenderChapter, ChapterRenderer>();
            services.AddTransient<IRenderFile, FileRenderer>();
            services.AddTransient<IRenderLibrary, LibraryRenderer>();
            services.AddTransient<IRenderLink, LinkRenderer>();
            services.AddTransient<IRenderSeries, SeriesRenderer>();
            AddCustomServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual IServiceCollection AddDatabaseConnection(IServiceCollection services)
        {
            services.AddDatabaseConnection();
            return services;
        }

        protected virtual IServiceCollection AddCustomServices(IServiceCollection services)
        {
            return services;
        }
    }
}
