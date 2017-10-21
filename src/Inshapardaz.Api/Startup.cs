using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Hangfire;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.AspNetCore;
using Paramore.Darker.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;

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
            RegisterRenderer(services);
            services.AddBrighter()
                    .AsyncHandlersFromAssemblies(typeof(Startup).Assembly)
                    .AsyncHandlersFromAssemblies(typeof(DomainMappingProfile).Assembly);
            services.AddDarker()
                    .AddHandlersFromAssemblies(typeof(DomainMappingProfile).Assembly);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "Inshapardaz API", Version = "v1" }); });

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration["ConnectionStrings:DefaultDatabase"]));

            services.AddCors();

            ConfigureApiAuthentication(services);
            ConfigureDomain(services);
            services.AddAuthorization();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api"))
                {
                    context.Request.Path = "/";
                    await next();
                }
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inshapardaz API"); });

            ConfigureObjectMappings(app);

            app.UseCors(policy => policy.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseStatusCodeMiddleWare();
            app.UseMvc();

            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }

        private void ConfigureApiAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = Configuration["Auth:Authority"];
                        options.ApiName = Configuration["Auth:Audience"];
                        options.RequireHttpsMetadata = false;
                    });
        }
        private void RegisterRenderer(IServiceCollection services)
        {
            services.AddTransient<IRenderResponse<EntryView>, EntryRenderer>();
            services.AddTransient<IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView>, DictionariesRenderer>();
            services.AddTransient<IRenderResponseFromObject<Dictionary, DictionaryView>, DictionaryRenderer>();
            services.AddTransient<IRenderEnum, EnumRenderer>();
            services.AddTransient<IRenderLink, LinkRenderer>();
            services.AddTransient<IRenderResponseFromObject<Word, WordView>, WordRenderer>();
            services.AddTransient<IRenderResponseFromObject<WordDetail, WordDetailView>, WordDetailRenderer>();
            services.AddTransient<IRenderResponseFromObject<Translation, TranslationView>, TranslationRenderer>();
            services.AddTransient<IRenderResponseFromObject<WordRelation, RelationshipView>, RelationRenderer>();
            services.AddTransient<IRenderResponseFromObject<WordDetail, IEnumerable<MeaningContextView>>, WordMeaningRenderer>();
            services.AddTransient<IRenderResponseFromObject<Meaning, MeaningView>, MeaningRenderer>();
            services.AddTransient<IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>>, WordIndexPageRenderer>();
            services.AddTransient<IRenderResponseFromObject<DownloadJobModel, DownloadDictionaryView>, DictionaryDownloadRenderer>();
            services.AddTransient<IRenderResponseFromObject<JobStatus, JobStatusModel>, JobStatusRenderer>();

        }

        public void ConfigureObjectMappings(IApplicationBuilder app)
        {
            Mapper.Initialize(c =>
                {
                    c.AddProfile(new MappingProfile());
                    c.AddProfile(new DomainMappingProfile());
                }
            );
            Mapper.AssertConfigurationIsValid();
        }

        private void ConfigureDomain(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:DefaultDatabase"];

            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DatabaseContext>(
                        options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
            services.AddTransient<IDatabaseContext, DatabaseContext>();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var database = new DatabaseContext(optionsBuilder.Options).Database;
            database.Migrate();
        }
    }
}
