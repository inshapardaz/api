
using System.IO;
using AutoMapper;
using Hangfire;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Database;
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

            ConfigureHangFire(services);
            
            services.AddCors();

            ConfigureApiAuthentication(services);
            ConfigureDomain(services);
            services.AddAuthorization();
            ConfigureMvc(services);
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
            ConfigureCustoMiddleWare(app);
            app.UseMvc();

            AddHangFire(app);
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

        protected virtual void ConfigureCustoMiddleWare(IApplicationBuilder app)
        {
            
        }

        protected virtual void ConfigureHangFire(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration["ConnectionStrings:DefaultDatabase"]));
        }

        protected virtual void AddHangFire(IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }

        protected virtual void ConfigureDomain(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:DefaultDatabase"];

            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DatabaseContext>(
                        options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
            services.AddTransient<IDatabaseContext, DatabaseContext>();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var database = new DatabaseContext(optionsBuilder.Options).Database;
            database.SetCommandTimeout(5 * 60);
            database.Migrate();
        }
    }
}
