﻿using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middleware;
using Inshapardaz.Api.Services;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Inshapardaz.Api.Converters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Database.SqlServer.Repositories;
using Inshapardaz.Api.Infrastructure;
using Inshapardaz.Adapter.Ocr.Google;

namespace Inshapardaz.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new Settings();
            Configuration.Bind("AppSettings", settings);
            services.AddSingleton(settings);

            services.AddDbContext<DataContext>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(settings.AllowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen();

            // configure DI for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddBrighterCommand();
            services.AddDarkerQuery();

            AddDatabaseConnection(services)
                .AddDatabase();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IUserHelper, UserHelper>();
            services.AddTransient<IRenderAccount, AccountRenderer>();
            services.AddTransient<IRenderAuthor, AuthorRenderer>();
            services.AddTransient<IRenderBook, BookRenderer>();
            services.AddTransient<IRenderCategory, CategoryRenderer>();
            services.AddTransient<IRenderChapter, ChapterRenderer>();
            services.AddTransient<IRenderFile, FileRenderer>();
            services.AddTransient<IRenderLibrary, LibraryRenderer>();
            services.AddTransient<IRenderLink, LinkRenderer>();
            services.AddTransient<IRenderSeries, SeriesRenderer>();
            services.AddTransient<IRenderPeriodical, PeriodicalRenderer>();
            services.AddTransient<IRenderIssue, IssueRenderer>();
            services.AddTransient<IRenderArticle, ArticleRenderer>();
            services.AddTransient<IRenderBookPage, BookPageRenderer>();
            services.AddTransient<IConvertPdf, PdfConverter>();
            services.AddTransient<IOpenZip, ZipReader>();
            services.AddTransient<IProvideOcr, GoogleOcr>();
            if (settings.FileStoreType == FileStoreTypes.AzureBlobStorage)
            {
                services.AddTransient<IFileStorage, AzureFileStorage>();
            }
            else
            {
                services.AddTransient<IFileStorage, DatabaseFileStorage>();
            }

            AddCustomServices(services);
        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            // migrate database changes on startup (includes initial db creation)
            //context.Database.Migrate();

            // generated swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Inshapardaz API"));

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseStaticFiles();

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseStatusCodeMiddleWare();
            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
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
