using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Inshapardaz.Adapter.Ocr.Google;
using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Infrastructure;
using Inshapardaz.Api.Infrastructure.Configuration;
using Inshapardaz.Api.Infrastructure.Factories;
using Inshapardaz.Api.Infrastructure.Middleware;
using Inshapardaz.Api.Infrastructure.Services;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query;
using Inshapardaz.Domain.Ports.Query.Library;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = RequestSizeLimits.Default);

const string serviceName = "Inshapardaz";

//=====================================================================
// Add services to the container.

// Configuration 
//--------------------------------------------------------------------
var configSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<Settings>(configSection);

builder.Host.UseSerilog((ctx, cfg) =>
{
    var config = cfg.Enrich.WithProperty("Application", serviceName)
        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);

    config
        .ReadFrom.Configuration(builder.Configuration)
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
        .MinimumLevel.Override("Paramore", LogEventLevel.Error)
        .WriteTo.Console();
});

//--------------------------------------------------------------------
// Cookie-based auth (see AccountsController's token/refreshToken cookies) means a CORS
// policy that both allows any origin and allows credentials would let any third-party
// site make authenticated requests on behalf of a logged-in user. So credentials are only
// allowed for origins explicitly listed in AppSettings:AllowedOrigins; everything else gets
// a permissive but credential-less policy (fine for anonymous/public reads).
var allowedOrigins = (configSection["AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        if (allowedOrigins.Length > 0)
        {
            policyBuilder.WithOrigins(allowedOrigins).AllowCredentials();
        }
        else
        {
            policyBuilder.SetIsOriginAllowed(_ => true);
        }

        policyBuilder.AllowAnyHeader()
               .AllowAnyMethod()
               .WithExposedHeaders(HeaderNames.Location, HeaderNames.ContentDisposition, HeaderNames.ContentType);
    });
});
builder.Services.AddControllers().AddJsonOptions(j =>
{
    j.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
});

// Throttles the unauthenticated account endpoints (login, password reset, invitations) per
// client IP so credential stuffing / brute force can't be run at full request speed. These
// endpoints have no other attempt-throttling (see issue #33).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.Configure<FormOptions>(x =>
{
    // Needs to allow up to RequestSizeLimits.BulkPageUpload -- the bulk page upload endpoints
    // (BookPageController/IssuePageController UploadPages) read the multipart form directly
    // rather than a bound model, so this is the limit that actually applies to them. Every
    // other FormOptions limit (ValueLengthLimit, MultipartBoundaryLengthLimit,
    // MultipartHeadersCountLimit/LengthLimit) is left at its framework default -- nothing in
    // this app needs, say, an oversized individual form field or header block.
    x.MultipartBodyLengthLimit = RequestSizeLimits.BulkPageUpload;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//-------------------------------------------------------------------
// Authentication & Authorization
//-------------------------------------------------------------------
var securitySettings = configSection.GetSection("Security").Get<Security>();
if (string.IsNullOrWhiteSpace(securitySettings.Secret))
{
    if (builder.Environment.IsDevelopment())
    {
        // Never fall back to a fixed secret -- a shared default would let anyone who has
        // read the source forge a token (including isSuperAdmin) for any deployment that
        // forgets to override it. Generating a random one here keeps `dotnet run` working
        // out of the box for local development; tokens just won't survive a restart.
        var ephemeralSecret = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        Console.Error.WriteLine("WARNING: AppSettings:Security:Secret is not configured -- using a random ephemeral secret for this run. " +
                     "Set the AppSettings__Security__Secret environment variable to keep issued tokens valid across restarts.");

        // builder.Configuration is a ConfigurationManager, so this write is visible to
        // everything that resolves IOptions<Settings> later (TokenGenerator, etc.) -- not
        // just the local `securitySettings` used below to set up JwtBearer validation.
        builder.Configuration["AppSettings:Security:Secret"] = ephemeralSecret;
        securitySettings = configSection.GetSection("Security").Get<Security>();
    }
    else
    {
        throw new InvalidOperationException("AppSettings:Security:Secret is not configured. Set the AppSettings__Security__Secret environment variable before starting the API.");
    }
}
var jwtKey = Encoding.ASCII.GetBytes(securitySettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = true,
        ValidIssuer = TokenGenerator.Issuer,
        ValidateAudience = true,
        ValidAudience = TokenGenerator.Audience,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Fall back to cookie if no Authorization header present
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["token"];
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

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
.AddTransient<IRenderCommonWord, CommonWordRenderer>()
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
    .AddScoped<IBookShelfRepository>(DatabaseFactory.GetBookShelfRepository)
    .AddScoped<ICommonWordsRepository>(DatabaseFactory.GetCommonWordsRepository);

//=====================================================================
var app = builder.Build();

var basePath = Environment.GetEnvironmentVariable("BASE_PATH");
if (!string.IsNullOrEmpty(basePath))
{
    Console.WriteLine($"Using base path {basePath}");
    app.UsePathBase(basePath);
}

// Configure the HTTP request pipeline.

// Swagger exposes the full API surface/schema, which makes reconnaissance easier for an
// attacker if left on in Production -- keep it to Development/other non-Production
// environments only. The docker healthcheck points at /health/check instead of the Swagger
// UI so it doesn't depend on this.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseRequestLogging();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<LibraryConfigurationMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
