using Inshapardaz.Api.Infrastructure;
using Inshapardaz.Api.Infrastructure.Factories;
using Inshapardaz.Api.Infrastructure.Services;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Inshapardaz.Adapter.Ocr.Google;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Converters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Domain.Ports.Query;
using Inshapardaz.Api.Infrastructure.Configuration;
using Inshapardaz.Api.Infrastructure.Middleware;
using Inshapardaz.Domain.Ports.Query.Library;
using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = null);

//=====================================================================
// Configure open telemetry

const string serviceName = "Inshapardaz";

string tracingOtlpEndpoint = builder.Configuration["OLTP_ENDPOINT_URL"];

if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddOpenTelemetry(options => options
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName))
        .AddConsoleExporter());

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter("Microsoft.AspnetCore.Hosting")
            .AddMeter("Microsoft.AspnetCore.Server.Kestrel")
            .AddPrometheusExporter()
            .AddConsoleExporter())
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            if (tracingOtlpEndpoint != null)
            {
                tracing.AddOtlpExporter(opt => opt.Endpoint = new Uri(tracingOtlpEndpoint));
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });
}

//=====================================================================
// Add services to the container.

// Configuration 
//--------------------------------------------------------------------
var configSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<Settings>(configSection);

//--------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins("*")
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

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartBoundaryLengthLimit = int.MaxValue;
    x.MultipartHeadersCountLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
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

var basePath = Environment.GetEnvironmentVariable("BASE_PATH");
if (!string.IsNullOrEmpty(basePath))
{
    Console.WriteLine($"Using base path {basePath}");
    app.UsePathBase(basePath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.MapPrometheusScrapingEndpoint();
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
