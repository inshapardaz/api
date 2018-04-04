﻿
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.IndexingService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        public Settings Settings { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);

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
            services.AddSingleton<IProvideIndexLocation, DictionaryIndexLocationProvider>();
            services.AddScoped<IWriteDictionaryIndex, DictionaryIndexWriter>();
            services.AddScoped<IReadDictionaryIndex, DictionaryIndexWriter>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Inshapardaz API", Version = "v1"}); });

            ConfigureHangFire(services);

            services.AddCors();

            ConfigureApiAuthentication(services);
            ConfigureDomain(services);
            ConfigureMvc(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine($"Configuring application with environment {env.EnvironmentName}");
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
            ConfigureCustomMiddleWare(app);
            app.UseMvc();

            AddHangFire(app);

            AddDomain(app);
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
            services.AddTransient<IRenderEnum, EnumRenderer>();
            services.AddTransient<IRenderLink, LinkRenderer>();
            services.AddTransient<IRenderEntry, EntryRenderer>();
            services.AddTransient<IRenderDictionaries, DictionariesRenderer>();
            services.AddTransient<IRenderDictionary, DictionaryRenderer>();
            services.AddTransient<IRenderWord, WordRenderer>();
            services.AddTransient<IRenderWordPage, WordPageRenderer>();
            services.AddTransient<IRenderMeaning, MeaningRenderer>();
            services.AddTransient<IRenderTranslation, TranslationRenderer>();
            services.AddTransient<IRenderRelation, RelationRenderer>();
            services.AddTransient<IRenderDictionaryDownload, DictionaryDownloadRenderer>();
            services.AddTransient<IRenderJobStatus, JobStatusRenderer>();
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

        protected virtual void ConfigureMvc(IServiceCollection services)
        {
            services.AddMvc();
        }

        protected virtual void ConfigureCustomMiddleWare(IApplicationBuilder app)
        {

        }

        protected virtual void ConfigureSettings(IServiceCollection services)
        {
            Settings = new Settings();
            Configuration.Bind("Application", Settings);
            services.AddSingleton(Settings);
        }

        protected virtual void ConfigureHangFire(IServiceCollection services)
        {
            //var connectionString = Configuration.GetConnectionString("DefaultDatabase");
            //Console.WriteLine($"Starting HangFire with connection string {connectionString}");
            //services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
        }

        protected virtual void AddHangFire(IApplicationBuilder app)
        {
            //app.UseHangfireServer();
            //app.UseHangfireDashboard();
        }

        protected virtual void ConfigureDomain(IServiceCollection services)
        {
            //var connectionString = Configuration.GetConnectionString("DefaultDatabase");

            //services.AddEntityFrameworkSqlServer()
            //        .AddDbContext<DatabaseContext>(
            //            options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
            //services.AddTransient<IDatabaseContext, DatabaseContext>();

            //bool.TryParse(Configuration["Application:RunDBMigrations"], out bool migrationEnabled);
            //if (migrationEnabled)
            //{
            //    Console.WriteLine("Running database migrations...");
            //    var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            //    optionsBuilder.UseSqlServer(connectionString);

            //    var database = new DatabaseContext(optionsBuilder.Options).Database;
            //    database.SetCommandTimeout(5 * 60);
            //    database.Migrate();
            //    Console.WriteLine("Database migrations completed.");
            //}

            services.AddTransient<IClientProvider, ClientProvider>();
            services.AddTransient<IInitialiseElasticSearch, ElasticsearchInitializer>();
            services.AddTransient<IProvideIndex, IndexProvider>();
        }

        private void AddDomain(IApplicationBuilder app)
        {
            var initialiser = app.ApplicationServices.GetService<IInitialiseElasticSearch>();
            initialiser.Initialise();
        }
    }
}