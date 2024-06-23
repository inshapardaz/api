using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Adapters.Database.SqlServer.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.TestHost;
using System;
using Inshapardaz.Api.Tests.Framework.Asserts;
using MailKit.Net.Smtp;
using System.IO;
using Microsoft.Extensions.Configuration;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Adapters.Database.SqlServer;
using Microsoft.Extensions.Options;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Api.Tests.Framework.DataHelpers;

namespace Inshapardaz.Api.Tests
{
    public class TestBase
    {
        public static DatabaseTypes DatabaseType => DatabaseTypes.MySql;
        protected readonly bool _periodicalsEnabled;
        protected readonly Role? _role;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AccountDto _account;

        protected AccountAssert AccountAssert => Services.GetService<AccountAssert>();
        protected FakeSmtpClient SmtpClient => Services.GetService<ISmtpClient>() as FakeSmtpClient;

        public TestBase(Role? role = null, bool periodicalsEnabled = false, bool createLibrary = true)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _role = role;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureTestServices(services => ConfigureServices(services));
            });

            var settings = Services.GetService<IOptions<Settings>>().Value;
            AccountBuilder = _factory.Services.GetService<AccountDataBuilder>();

            if (role.HasValue)
            {
                AccountBuilder = AccountBuilder.As(_role.Value).Verified();
                _account = AccountBuilder.Build();
            }

            if (createLibrary)
            {
                var builder = LibraryBuilder.WithPeriodicalsEnabled(_periodicalsEnabled);
                if (_account != null && role.HasValue) builder.AssignToUser(AccountId, _role.Value);
                Library = builder.Build();
            }

            Client = _factory.CreateClient();

            if (_account != null)
            {
                var token = TokenBuilder.GenerateToken(settings, _account.Id);
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            if (services.Any(x => x.ServiceType == typeof(IFileStorage)))
            {
                services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(SqlServerDatabaseFileStorage), ServiceLifetime.Transient));
                services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(AzureFileStorage), ServiceLifetime.Transient));
            }
            services.AddSingleton<IFileStorage>(new FakeFileStorage());
            services.AddTransient<FakeFileStorage>(sp => sp.GetService<IFileStorage>() as FakeFileStorage);

            if (services.Any(x => x.ServiceType == typeof(ISmtpClient)))
            {
                services.Remove(new ServiceDescriptor(typeof(ISmtpClient), typeof(SmtpClient), ServiceLifetime.Transient));
            }
            services.AddSingleton<ISmtpClient>(new FakeSmtpClient());
            services.AddTransient<FakeSmtpClient>(sp => sp.GetService<ISmtpClient>() as FakeSmtpClient);


            if (DatabaseType == DatabaseTypes.SqlServer)
            {
                services.AddTransient<IProvideConnection, SqlServerConnectionProvider>();
                services.AddTransient<IAccountTestRepository, SqlServerAccountTestRepository>();
                services.AddTransient<ILibraryTestRepository, SqlServerLibraryTestRepository>();
                services.AddTransient<IArticleTestRepository, SqlServerArticleTestRepository>();
                services.AddTransient<IAuthorTestRepository, SqlServerAuthorTestRepository>();
                services.AddTransient<IBookTestRepository, SqlServerBookTestRepository>();
                services.AddTransient<IBookPageTestRepository, SqlServerBookPageTestRepository>();
                services.AddTransient<IBookShelfTestRepository, SqlServerBookShelfTestRepository>();
                services.AddTransient<IChapterTestRepository, SqlServerChapterTestRepository>();
                services.AddTransient<ICategoryTestRepository, SqlServerCategoryTestRepository>();
                services.AddTransient<ISeriesTestRepository, SqlServerSeriesTestRepository>();
                services.AddTransient<IPeriodicalTestRepository, SqlServerPeriodicalTestRepository>();
                services.AddTransient<IIssueTestRepository, SqlServerIssueTestRepository>();
                services.AddTransient<IIssuePageTestRepository, SqlServerIssuePageTestRepository>();
                services.AddTransient<IIssueArticleTestRepository, SqlServerIssueArticleTestRepository>();
                services.AddTransient<IFileTestRepository, SqlServerFileTestRepository>();
                services.AddTransient<ICorrectionTestRepository, SqlServerCorrectionTestRepository>();
            }
            else if (DatabaseType == DatabaseTypes.MySql)
            {
                services.AddTransient<IProvideConnection, MySqlConnectionProvider>();
                services.AddTransient<IAccountTestRepository, MySqlAccountTestRepository>();
                services.AddTransient<ILibraryTestRepository, MySqlLibraryTestRepository>();
                services.AddTransient<IArticleTestRepository, MySqlArticleTestRepository>();
                services.AddTransient<IAuthorTestRepository, MySqlAuthorTestRepository>();
                services.AddTransient<IBookTestRepository, MySqlBookTestRepository>();
                services.AddTransient<IBookPageTestRepository, MySqlBookPageTestRepository>();
                services.AddTransient<IBookShelfTestRepository, MySqlBookShelfTestRepository>();
                services.AddTransient<IChapterTestRepository, MySqlChapterTestRepository>();
                services.AddTransient<ICategoryTestRepository, MySqlCategoryTestRepository>();
                services.AddTransient<ISeriesTestRepository, MySqlSeriesTestRepository>();
                services.AddTransient<IPeriodicalTestRepository, MySqlPeriodicalTestRepository>();
                services.AddTransient<IIssueTestRepository, MySqlIssueTestRepository>();
                services.AddTransient<IIssuePageTestRepository, MySqlIssuePageTestRepository>();
                services.AddTransient<IIssueArticleTestRepository, MySqlIssueArticleTestRepository>();
                services.AddTransient<IFileTestRepository, MySqlFileTestRepository>();
                services.AddTransient<ICorrectionTestRepository, MySqlCorrectionTestRepository>();
            }
            else
            {
                throw new Exception($"Database type {DatabaseType} is not supported.");
            }

            services.AddTransient<LibraryDataBuilder>()
            .AddTransient<CategoriesDataBuilder>()
            .AddTransient<SeriesDataBuilder>()
            .AddTransient<AuthorsDataBuilder>()
            .AddTransient<BooksDataBuilder>()
            .AddTransient<ChapterDataBuilder>()
            .AddTransient<AccountDataBuilder>()
            .AddTransient<PeriodicalsDataBuilder>()
            .AddTransient<IssueDataBuilder>()
            .AddTransient<AccountAssert>()
            .AddTransient<CorrectionBuilder>()
            .AddTransient<BookShelfDataBuilder>()
            .AddTransient<ArticlesDataBuilder>()
            .AddTransient<FileStoreAssert>();

            services.AddTransient<ArticleAssert>()
                    .AddTransient<ArticleAssert>()
                    .AddTransient<ArticleContentAssert>()
                    .AddTransient<AuthorAssert>()
                    .AddTransient<BookAssert>()
                    .AddTransient<BookContentAssert>()
                    .AddTransient<BookPageAssert>()
                    .AddTransient<BookShelfAssert>()
                    .AddTransient<CategoryAssert>()
                    .AddTransient<ChapterAssert>()
                    .AddTransient<ChapterContentAssert>()
                    .AddTransient<CorrectionAssert>()
                    .AddTransient<FileStoreAssert>()
                    .AddTransient<IssueArticleAssert>()
                    .AddTransient<IssueArticleContentAssert>()
                    .AddTransient<IssueAssert>()
                    .AddTransient<IssueContentAssert>()
                    .AddTransient<IssuePageAssert>()
                    .AddTransient<LibraryAssert>()
                    .AddTransient(typeof(PagingAssert<>), typeof(PagingAssert<>))
                    .AddTransient<PeriodicalAssert>()
                    .AddTransient<SeriesAssert>();
        }

        protected void AuthenticateClientWithToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public HttpClient Client { get; }

        public int AccountId => _account?.Id ?? 0;

        protected AccountDto Account => _account;

        protected int LibraryId => _libraryBuilder.Library.Id;

        protected IServiceProvider Services => _factory.Services;

        protected LibraryDto Library { get; }

        protected FakeFileStorage FileStore => _factory.Services.GetService<IFileStorage>() as FakeFileStorage;

        protected AccountDataBuilder _accountBuilder;
        protected LibraryDataBuilder _libraryBuilder;
        protected AuthorsDataBuilder _authorBuilder;
        protected SeriesDataBuilder _seriesDataBuilder;
        protected CategoriesDataBuilder _categoriesDataBuilder;
        protected BooksDataBuilder _booksDataBuilder;
        protected ChapterDataBuilder _chapterDataBuilder;
        protected IssueDataBuilder _issueDataBuilder;
        private PeriodicalsDataBuilder _periodicalBuilder;
        protected CorrectionBuilder _correctionBuilder;
        private BookShelfDataBuilder _bookshelfDataBuilder;
        private ArticlesDataBuilder _articleBuilder;
        private FileStoreAssert _fileStoreAssert;

        protected Role? CurrentAuthenticationLevel => _role;

        protected AccountDataBuilder AccountBuilder { get; }

        protected LibraryDataBuilder LibraryBuilder
        {
            get
            {
                if (_libraryBuilder == null)
                {
                    _libraryBuilder = _factory.Services.GetService<LibraryDataBuilder>();
                }

                return _libraryBuilder;
            }
        }

        protected AuthorsDataBuilder AuthorBuilder
        {
            get
            {
                if (_authorBuilder == null)
                {
                    _authorBuilder = _factory.Services.GetService<AuthorsDataBuilder>();
                }

                return _authorBuilder;
            }
        }

        protected SeriesDataBuilder SeriesBuilder
        {
            get
            {
                if (_seriesDataBuilder == null)
                {
                    _seriesDataBuilder = _factory.Services.GetService<SeriesDataBuilder>();
                }

                return _seriesDataBuilder;
            }
        }

        protected CategoriesDataBuilder CategoryBuilder
        {
            get
            {
                if (_categoriesDataBuilder == null)
                {
                    _categoriesDataBuilder = _factory.Services.GetService<CategoriesDataBuilder>();
                }

                return _categoriesDataBuilder;
            }
        }

        protected BooksDataBuilder BookBuilder
        {
            get
            {
                if (_booksDataBuilder == null)
                {
                    _booksDataBuilder = _factory.Services.GetService<BooksDataBuilder>();
                }

                return _booksDataBuilder;
            }
        }

        protected ArticlesDataBuilder ArticleBuilder
        {
            get
            {
                if (_articleBuilder == null)
                {
                    _articleBuilder = _factory.Services.GetService<ArticlesDataBuilder>();
                }

                return _articleBuilder;
            }
        }

        protected BookShelfDataBuilder BookShelfBuilder
        {
            get
            {
                if (_bookshelfDataBuilder == null)
                {
                    _bookshelfDataBuilder = _factory.Services.GetService<BookShelfDataBuilder>();
                }

                return _bookshelfDataBuilder;
            }
        }

        protected PeriodicalsDataBuilder PeriodicalBuilder
        {
            get
            {
                if (_periodicalBuilder == null)
                {
                    _periodicalBuilder = _factory.Services.GetService<PeriodicalsDataBuilder>();
                }

                return _periodicalBuilder;
            }
        }

        protected ChapterDataBuilder ChapterBuilder
        {
            get
            {
                if (_chapterDataBuilder == null)
                {
                    _chapterDataBuilder = _factory.Services.GetService<ChapterDataBuilder>();
                }

                return _chapterDataBuilder;
            }
        }

        protected IssueDataBuilder IssueBuilder
        {
            get
            {
                if (_issueDataBuilder == null)
                {
                    _issueDataBuilder = _factory.Services.GetService<IssueDataBuilder>();
                }

                return _issueDataBuilder;
            }
        }

        protected CorrectionBuilder CorrectionBuilder
        {
            get
            {
                if (_correctionBuilder == null)
                {
                    _correctionBuilder = _factory.Services.GetService<CorrectionBuilder>();
                }

                return _correctionBuilder;
            }
        }

        protected FileStoreAssert FileAssert
        {
            get
            {
                if (_fileStoreAssert == null)
                {
                    _fileStoreAssert = _factory.Services.GetService<FileStoreAssert>();
                }

                return _fileStoreAssert;
            }
        }

        protected IAccountTestRepository AccountTestRepository => _factory.Services.GetService<IAccountTestRepository>();
        protected IArticleTestRepository ArticleTestRepository => _factory.Services.GetService<IArticleTestRepository>();
        protected IAuthorTestRepository AuthorTestRepository => _factory.Services.GetService<IAuthorTestRepository>();
        protected ILibraryTestRepository LibraryTestRepository => _factory.Services.GetService<ILibraryTestRepository>();
        protected IBookTestRepository BookTestRepository => _factory.Services.GetService<IBookTestRepository>();
        protected IBookPageTestRepository BookPageTestRepository => _factory.Services.GetService<IBookPageTestRepository>();
        protected IBookShelfTestRepository BookShelfTestRepository => _factory.Services.GetService<IBookShelfTestRepository>();
        protected IChapterTestRepository ChapterTestRepository => _factory.Services.GetService<IChapterTestRepository>();
        protected ICategoryTestRepository CategoryTestRepository => _factory.Services.GetService<ICategoryTestRepository>();
        protected ISeriesTestRepository SeriesTestRepository => _factory.Services.GetService<ISeriesTestRepository>();
        protected IPeriodicalTestRepository PeriodicalTestRepository => _factory.Services.GetService<IPeriodicalTestRepository>();
        protected IIssueTestRepository IssueTestRepository => _factory.Services.GetService<IIssueTestRepository>();
        protected IIssuePageTestRepository IssuePageTestRepository => _factory.Services.GetService<IIssuePageTestRepository>();
        protected IIssueArticleTestRepository IssueArticleTestRepository => _factory.Services.GetService<IIssueArticleTestRepository>();
        protected IFileTestRepository FileTestRepository => _factory.Services.GetService<IFileTestRepository>();
        protected ICorrectionTestRepository CorrectionTestRepository => _factory.Services.GetService<ICorrectionTestRepository>();

        protected virtual void Cleanup()
        {
            _bookshelfDataBuilder?.CleanUp();
            _chapterDataBuilder?.CleanUp();
            _booksDataBuilder?.CleanUp();
            _seriesDataBuilder?.CleanUp();
            _issueDataBuilder?.CleanUp();
            _periodicalBuilder?.CleanUp();
            _articleBuilder?.CleanUp();
            _accountBuilder?.CleanUp();
            _authorBuilder?.CleanUp();
            _categoriesDataBuilder?.CleanUp();
            _libraryBuilder?.CleanUp();
            _correctionBuilder?.Cleanup();
        }
    }
}
