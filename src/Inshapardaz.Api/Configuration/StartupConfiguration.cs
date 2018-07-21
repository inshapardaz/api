﻿using System.IO;
using AutoMapper;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.IndexingService;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Repositories;
using Inshapardaz.Ports.Database.Repositories.Dictionary;
using Inshapardaz.Ports.Database.Repositories.Library;
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
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Inshapardaz.Api.Configuration
{
    public static class StartupConfiguration
    {

        public static Settings BindSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.Bind("Application", settings);
            services.AddSingleton(settings);

            return settings;
        }

        public static IServiceCollection AddBrighterAndDarker(this IServiceCollection services)
        {
            services.AddBrighter()
                    .AsyncHandlersFromAssemblies(typeof(Startup).Assembly)
                    .AsyncHandlersFromAssemblies(typeof(Settings).Assembly);
            services.AddDarker()
                    .AddHandlersFromAssemblies(typeof(Settings).Assembly);

            return services;
        }

        public static IServiceCollection AddRenderers(this IServiceCollection services)
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
            services.AddTransient<IRenderFile, FileRenderer>();

            services.AddTransient<IRenderGenre, GenreRenderer>();
            services.AddTransient<IRenderGenres, GenresRenderer>();
            services.AddTransient<IRenderAuthors, AuthorsRenderer>();
            services.AddTransient<IRenderAuthor, AuthorRenderer>();
            services.AddTransient<IRenderBooks, BooksRenderer>();
            services.AddTransient<IRenderBook, BookRenderer>();

            return services;
        }

        public static IServiceCollection AddFramework(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddScoped<IUserHelper, UserHelper>();

            return services;
        }

        public static IServiceCollection AddLucene(this IServiceCollection services)
        {
            services.AddSingleton<IProvideIndexLocation, DictionaryIndexLocationProvider>();
            services.AddScoped<IWriteDictionaryIndex, DictionaryIndexWriter>();
            services.AddScoped<IReadDictionaryIndex, DictionaryIndexWriter>();

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "Inshapardaz API", Version = "v1" }); });
            return services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services)
        {
            //app.UseHangfireServer();
            //app.UseHangfireDashboard();
            return services;
        }

        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, Settings settings)
        {
            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = settings.Authority;
                        options.ApiName = settings.Audience;
                        options.RequireHttpsMetadata = false;
                    });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            /*if (hostingEnvironment.IsEnvironment("Test"))
            {
                services.AddDbContext<DatabaseContext>(options =>
                {
                    var connection = new SqliteConnection("DataSource='file::memory:?cache=shared'");
                    connection.Open();
                    options.UseSqlite(connection);
                });

                services.AddSingleton<IDatabaseContext, DatabaseContext>();
            }
            else*/
            {
                var connectionString = configuration.GetConnectionString("DefaultDatabase");
                services.AddEntityFrameworkSqlServer()
                        .AddDbContext<DatabaseContext>(
                            options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));

                services.AddTransient<IDatabaseContext, DatabaseContext>();
            }

            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IDictionaryRepository, DictionaryRepository>();
            services.AddTransient<IWordRepository, WordRepository>();
            services.AddTransient<IMeaningRepository, MeaningRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<IRelationshipRepository, RelationshipRepository>();

            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IChapterRepository, ChapterRepository>();

            return services;
        }

        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            Mapper.Initialize(c =>
                {
                    c.AddProfile(new MappingProfile());
                    c.AddProfile(new DatabaseMappingProfile());
                }
            );
            Mapper.AssertConfigurationIsValid();

            return services;
        }
        
        public static IApplicationBuilder UseApiRedirection(this IApplicationBuilder app)
        {
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

            return app;
        }

        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inshapardaz API"); });
            return app;
        }

        public static IApplicationBuilder UseCors(this IApplicationBuilder app)
        {
            app.UseCors(policy => policy.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());
            return app;
        }

        public static IApplicationBuilder UseTestAuthentication(this IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsEnvironment("Test"))
            {
                app.UseMiddleware<TestAuthenticatedRequestMiddleware>();
            }

            return app;
        }



    }
}