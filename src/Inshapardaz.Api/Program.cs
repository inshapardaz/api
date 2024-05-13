using Inshapardaz.Api.Infrastructure;
using Inshapardaz.Api.Infrastructure.Factories;
using Inshapardaz.Api.Infrastructure.Services;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using Inshapardaz.Api.Configuration;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Inshapardaz.Adapter.Ocr.Google;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Converters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Api.Middleware;
using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Adapters.Database.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Inshapardaz.Domain.Ports.Query;

var builder = WebApplication.CreateBuilder(args);


//=====================================================================
// Add services to the container.

// Configuration 
//--------------------------------------------------------------------
var configSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<Settings>(configSection);

//--------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .WithExposedHeaders(HeaderNames.Location, HeaderNames.ContentDisposition, HeaderNames.ContentType);
    });
});
builder.Services.AddControllers().AddJsonOptions(j =>
{
    j.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//-------------------------------------------------------------------
builder.Services.AddTransient<DatabaseMigrationFactory>();
builder.Services.AddHostedService<MigrationService>();

//------------------------------------------------------------------
builder.Services.AddTransient<ISmtpClient, SmtpClient>()
    .AddSingleton<ISendEmail, EmailSender>()
    .AddSingleton<IGetIPAddress, HttpIPAddressGetter>()
    .AddTransient<IGenerateToken, TokenGenerator>();
//------------------------------------------------------------------

builder.Services.AddAutoMapper(typeof(Program).Assembly);
//------------------------------------------------------------------

builder.Services.AddBrighterCommand();
builder.Services.AddDarkerQuery();
//------------------------------------------------------------------

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
.AddTransient<IUserHelper, UserHelper>()
.AddTransient<IRenderAccount, AccountRenderer>()
.AddTransient<IRenderAuthor, AuthorRenderer>()
.AddTransient<IRenderBook, BookRenderer>()
.AddTransient<IRenderCategory, CategoryRenderer>()
.AddTransient<IRenderChapter, ChapterRenderer>()
.AddTransient<IRenderFile, FileRenderer>()
.AddTransient<IRenderLibrary, LibraryRenderer>()
.AddTransient<IRenderLink, LinkRenderer>()
.AddTransient<IRenderSeries, SeriesRenderer>()
.AddTransient<IRenderPeriodical, PeriodicalRenderer>()
.AddTransient<IRenderArticle, ArticleRenderer>()
.AddTransient<IRenderIssue, IssueRenderer>()
.AddTransient<IRenderIssuePage, IssuePageRenderer>()
.AddTransient<IRenderIssueArticle, IssueArticleRenderer>()
.AddTransient<IRenderBookPage, BookPageRenderer>()
.AddTransient<IRenderCorrection, CorrectionRenderer>()
.AddTransient<IRenderBookSelf, BookShelfRenderer>()
.AddTransient<IConvertPdf, PdfConverter>()
.AddTransient<IOpenZip, ZipReader>()
.AddTransient<IProvideOcr, GoogleOcr>()
.AddScoped<LibraryConfiguration>()
.AddTransient<IWriteWordDocument, WordDocumentWriter>()
.AddScoped(typeof(AuthorizeAdminDecorator<,>))
.AddScoped(typeof(LibraryAuthorizeDecorator<,>))
.AddScoped(FileStorageFactory.GetFileStore);
//AddCustomServices(services)

//------------------------------------------------------------------
builder.Services.AddSqlServer()
    .AddMySql()
    .AddScoped<ILibraryRepository>(DatabaseFactory.GetLibraryRepository)
    .AddScoped<SqlServerConnectionProvider>()
    .AddScoped<MySqlConnectionProvider>()
    .AddScoped<IFileRepository>(DatabaseFactory.GetFileRepository)
    .AddScoped<IAuthorRepository>(DatabaseFactory.GetAuthorRepository)
    .AddScoped<IArticleRepository>(DatabaseFactory.GetArticleRepository)
    .AddScoped<ICategoryRepository>(DatabaseFactory.GetCategoryRepository)
    .AddScoped<IBookRepository>(DatabaseFactory.GetBookRepository)
    .AddScoped<IChapterRepository>(DatabaseFactory.GetChapterRepository)
    .AddScoped<ISeriesRepository>(DatabaseFactory.GetSeriesRepository)
    .AddScoped<IBookPageRepository>(DatabaseFactory.GetBookPageRepository)
    .AddScoped<IPeriodicalRepository>(DatabaseFactory.GetPeriodicalRepository)
    .AddScoped<IIssueRepository>(DatabaseFactory.GetIssueRepository)
    .AddScoped<IIssueArticleRepository>(DatabaseFactory.GetIssueArticleRepository)
    .AddScoped<IAccountRepository>(DatabaseFactory.GetAccountRepository)
    .AddScoped<ICorrectionRepository>(DatabaseFactory.GetCorrectionRepository)
    .AddScoped<IIssuePageRepository>(DatabaseFactory.GetIssuePageRepository)
    .AddScoped<IBookShelfRepository>(DatabaseFactory.GetBookShelfRepository);

//=====================================================================
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders(HeaderNames.Location, HeaderNames.ContentDisposition, HeaderNames.ContentType));

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRequestLogging();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<LibraryConfigurationMiddleware>();
app.UseStatusCodeMiddleWare();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
