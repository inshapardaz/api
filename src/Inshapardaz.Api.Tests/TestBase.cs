using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Database.SqlServer.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;
using System.Net.Http;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.TestHost;
using System;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Api.Tests.Asserts;
using MailKit.Net.Smtp;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Inshapardaz.Api.Tests
{
    public class TestBase
    {
        protected readonly bool _periodicalsEnabled;
        protected readonly Role? _role;
        private WebApplicationFactory<Startup> _factory;
        private AccountDto _account;

        protected AccountAssert AccountAssert => Services.GetService<AccountAssert>();
        public FakeSmtpClient SmtpClient => Services.GetService<ISmtpClient>() as FakeSmtpClient;

        public TestBase(Role? role = null, bool periodicalsEnabled = false, bool createLibrary = true)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _role = role;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            _factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);

                    if (!string.IsNullOrWhiteSpace(environment))
                    {
                        conf.AddJsonFile(Path.Combine(projectDir, $"appsettings.json"), true);
                        //conf.AddJsonFile(Path.Combine(projectDir, $"appsettings.{environment}.json"), true);
                    }
                });
                builder.ConfigureTestServices(services => ConfigureServices(services));
            });

            var settings = Services.GetService<Settings>();
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
                services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(DatabaseFileStorage), ServiceLifetime.Transient));
                services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(AzureFileStorage), ServiceLifetime.Transient));
            }
            services.AddSingleton<IFileStorage>(new FakeFileStorage());

            if (services.Any(x => x.ServiceType == typeof(ISmtpClient)))
            {
                services.Remove(new ServiceDescriptor(typeof(ISmtpClient), typeof(SmtpClient), ServiceLifetime.Transient));
            }
            services.AddSingleton<ISmtpClient>(new FakeSmtpClient());

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
                .AddTransient<BookShelfDataBuilder>();
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

        protected IDbConnection DatabaseConnection => _factory.Services.GetService<IProvideConnection>().GetConnection();

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
        protected virtual void Cleanup()
        {
            _accountBuilder?.CleanUp();
            _authorBuilder?.CleanUp();
            _seriesDataBuilder?.CleanUp();
            _categoriesDataBuilder?.CleanUp();
            _booksDataBuilder?.CleanUp();
            _chapterDataBuilder?.CleanUp();
            _periodicalBuilder?.CleanUp();
            _issueDataBuilder?.CleanUp();
            _accountBuilder?.CleanUp();
            _libraryBuilder?.CleanUp();
            _correctionBuilder?.Cleanup();
            _bookshelfDataBuilder?.CleanUp();
        }
    }
}
